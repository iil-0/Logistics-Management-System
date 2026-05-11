using System.Collections.Concurrent;

namespace LogiTechAPI.Command
{
    // Singleton Invoker — her kullanıcı için ayrı undo/redo stack tutar.
    // Per-user stack sayesinde Kullanıcı A, Kullanıcı B'nin işlemini
    // undo edemez. ConcurrentDictionary + lock ile thread-safe.
    public class KargoCommandInvoker
    {
        private readonly ConcurrentDictionary<int, UserHistory> _histories = new();

        private class UserHistory
        {
            public Stack<IKargoCommand> UndoStack { get; } = new();
            public Stack<IKargoCommand> RedoStack { get; } = new();
            public object Lock { get; } = new();
        }

        private UserHistory GetHistory(int userId)
            => _histories.GetOrAdd(userId, _ => new UserHistory());

        // Yeni komut çalıştırıldığında redo stack temizlenir — bu standart
        // davranıştır (Word/Photoshop gibi): undo'dan sonra yeni iş yapılırsa
        // redo geçmişi anlamsızlaşır.
        public async Task<KomutSonuc> ExecuteCommand(int userId, IKargoCommand command)
        {
            var result = await command.Execute();
            if (result.Basarili)
            {
                var history = GetHistory(userId);
                lock (history.Lock)
                {
                    history.UndoStack.Push(command);
                    history.RedoStack.Clear();
                }
            }
            return result;
        }

        public async Task<KomutSonuc> UndoLastCommand(int userId)
        {
            var history = GetHistory(userId);
            IKargoCommand command;
            lock (history.Lock)
            {
                if (history.UndoStack.Count == 0)
                    return new KomutSonuc { Basarili = false, Mesaj = "Geri alınacak işlem yok." };
                command = history.UndoStack.Pop();
                history.RedoStack.Push(command);
            }
            return await command.Undo();
        }

        public async Task<KomutSonuc> RedoLastCommand(int userId)
        {
            var history = GetHistory(userId);
            IKargoCommand command;
            lock (history.Lock)
            {
                if (history.RedoStack.Count == 0)
                    return new KomutSonuc { Basarili = false, Mesaj = "Yapılacak yeniden işlem yok." };
                command = history.RedoStack.Pop();
                history.UndoStack.Push(command);
            }
            return await command.Execute();
        }

        // Frontend butonların aktif/pasif olmasını belirlemek için kullanılır.
        public (bool canUndo, bool canRedo) GetStatus(int userId)
        {
            var history = GetHistory(userId);
            lock (history.Lock)
            {
                return (history.UndoStack.Count > 0, history.RedoStack.Count > 0);
            }
        }
    }
}
