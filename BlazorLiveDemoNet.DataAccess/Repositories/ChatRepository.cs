using BlazorLiveDemoNet.DataAccess.Models;
using BlazorLiveDemoNet.Shared.DTOs;
using MongoDB.Driver;

namespace BlazorLiveDemoNet.DataAccess.Repositories;

public class ChatRepository
{
    private readonly IMongoCollection<ChatMessageModel> _chatMessages;

    public ChatRepository()
    {
        var host = "localhost";
        var databaseName = "NetChat";
        var port = 27017;
        var connectionString = $"mongodb://{host}:{port}";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _chatMessages =
            database.GetCollection<ChatMessageModel>("Messages",
                new MongoCollectionSettings() {AssignIdOnInsert = true});
    }

    public async Task AddAsync(ChatMessageDto dto)
    {
        await _chatMessages.InsertOneAsync(ConvertToModel(dto));
    }

    public async Task<ChatMessageDto[]> GetAllMessages()
    {
        var filter = Builders<ChatMessageModel>.Filter.Empty;
        var all = await _chatMessages.FindAsync(filter);

        return all.ToList().Select(ConvertToDto).ToArray();
    }

    private ChatMessageModel ConvertToModel(ChatMessageDto dto)
    {
        return new ChatMessageModel()
        {
            SenderName = dto.Name,
            Message = dto.Message,
            TimeSent = dto.Timestamp
        };
    }

    private ChatMessageDto ConvertToDto(ChatMessageModel dataModel)
    {
        return new ChatMessageDto()
        {
            Name = dataModel.SenderName,
            Message = dataModel.Message,
            Timestamp = dataModel.TimeSent
        };
    }
}