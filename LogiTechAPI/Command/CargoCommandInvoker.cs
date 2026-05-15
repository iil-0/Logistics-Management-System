// Command pattern'in INVOKER'ı. Komutları çalıştırır ve undo/redo geçmişini tutar.
// Hangi komutu çalıştırdığını BİLMEZ; sadece ICargoCommand sözleşmesi üzerinden konuşur.
// Singleton — DI'da AddSingleton ile kayıtlı (Program.cs).
using System.Collections.Concurrent;

namespace LogiTechAPI.Command
{
    public class CargoCommandInvoker
    {
        // Her kullanıcı için ayrı undo/redo stack — A kullanıcısı B'nin işlemini undo edemez.
        // ConcurrentDictionary + lock ile thread-safe (Singleton paylaşılan state).
        private readonly ConcurrentDictionary<int, UserHistory> _histories = new();

        private class UserHistory
        {
            public Stack<ICargoCommand> UndoStack { get; } = new();   // LIFO: son komut ilk geri alınır
            public Stack<ICargoCommand> RedoStack { get; } = new();   // Undo edilen komutlar buraya itilir
            public object Lock { get; } = new();
        }

        private UserHistory GetHistory(int userId)
            => _histories.GetOrAdd(userId, _ => new UserHistory());

        public async Task<CommandResult> ExecuteCommand(int userId, ICargoCommand command)
        {
            var result = await command.Execute();
            if (result.Success)
            {
                var history = GetHistory(userId);
                lock (history.Lock)
                {
                    history.UndoStack.Push(command);
                    history.RedoStack.Clear();    // Yeni işlem → eski redo geçmişi anlamsızlaşır
                }
            }
            return result;
        }

        public async Task<CommandResult> UndoLastCommand(int userId)
        {
            var history = GetHistory(userId);
            ICargoCommand command;
            lock (history.Lock)
            {
                if (history.UndoStack.Count == 0)
                    return new CommandResult { Success = false, Message = "Geri alınacak işlem yok." };
                command = history.UndoStack.Pop();
                history.RedoStack.Push(command);  // Redo için sakla
            }
            return await command.Undo();          // Lock dışında çağrı — DB IO'yu kilitle bloklama
        }

        public async Task<CommandResult> RedoLastCommand(int userId)
        {
            var history = GetHistory(userId);
            ICargoCommand command;
            lock (history.Lock)
            {
                if (history.RedoStack.Count == 0)
                    return new CommandResult { Success = false, Message = "Yapılacak yeniden işlem yok." };
                command = history.RedoStack.Pop();
                history.UndoStack.Push(command);  // Tekrar undo edilebilir hale getir
            }
            return await command.Execute();
        }

        // Frontend buton aktif/pasif kararı için
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
