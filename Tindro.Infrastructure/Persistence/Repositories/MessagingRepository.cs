namespace Tindro.Infrastructure.Persistence.Repositories;

using Tindro.Application.Chat.Interfaces;
using Tindro.Domain.Chat;
using Microsoft.EntityFrameworkCore;

public class MessagingRepository : IMessagingRepository
{
    private readonly CommandDbContext _context;

    public MessagingRepository(CommandDbContext context)
    {
        _context = context;
    }

    

    public async Task<MessageReadReceipt> CreateReceiptAsync(MessageReadReceipt receipt)
    {
        _context.MessageReadReceipts.Add(receipt);
        await _context.SaveChangesAsync();
        return receipt;
    }

    


    

    public async Task<TypingIndicator> CreateTypingIndicatorAsync(TypingIndicator indicator)
    {
        _context.TypingIndicators.Add(indicator);
        await _context.SaveChangesAsync();
        return indicator;
    }

    public async Task UpdateTypingIndicatorAsync(TypingIndicator indicator)
    {
        _context.TypingIndicators.Update(indicator);
        await _context.SaveChangesAsync();
    }

    

    public async Task RemoveTypingIndicatorAsync(Guid indicatorId)
    {
        var indicator = await _context.TypingIndicators.FindAsync(indicatorId);
        if (indicator != null)
        {
            _context.TypingIndicators.Remove(indicator);
            await _context.SaveChangesAsync();
        }
    }

    // Voice notes


    public async Task<VoiceNote> CreateVoiceNoteAsync(VoiceNote voiceNote)
    {
        _context.VoiceNotes.Add(voiceNote);
        await _context.SaveChangesAsync();
        return voiceNote;
    }

    public async Task UpdateVoiceNoteAsync(VoiceNote voiceNote)
    {
        _context.VoiceNotes.Update(voiceNote);
        await _context.SaveChangesAsync();
    }


    

    public async Task<MessageExtension> CreateMessageExtensionAsync(MessageExtension extension)
    {
        _context.MessageExtensions.Add(extension);
        await _context.SaveChangesAsync();
        return extension;
    }

    public async Task UpdateMessageExtensionAsync(MessageExtension extension)
    {
        extension.ReadByCount = extension.ReadReceipts?.Count ?? 0;
        _context.MessageExtensions.Update(extension);
        await _context.SaveChangesAsync();
    }


    public async Task<ConversationSettings> CreateConversationSettingsAsync(ConversationSettings settings)
    {
        _context.ConversationSettings.Add(settings);
        await _context.SaveChangesAsync();
        return settings;
    }

    public async Task UpdateConversationSettingsAsync(ConversationSettings settings)
    {
        _context.ConversationSettings.Update(settings);
        await _context.SaveChangesAsync();
    }
}
