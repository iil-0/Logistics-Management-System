namespace LogiTechAPI.Command
{
    public class KargoCommandInvoker
    {
        private readonly Stack<IKargoCommand> _undoStack = new();

        public async Task<KomutSonuc> ExecuteCommand(IKargoCommand command)
        {
            var result = await command.Execute();
            if (result.Basarili)
            {
                _undoStack.Push(command);
            }
            return result;
        }

        public async Task<KomutSonuc> UndoLastCommand()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                return await command.Undo();
            }
            return new KomutSonuc { Basarili = false, Mesaj = "No command to undo." };
        }
    }
}
