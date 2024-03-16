using Game.Core.Managers;
using Injection;

namespace Game.Managers
{
    public struct SessionInfo
    {
        public int CountSession;
    }

    public sealed class SessionManager : Manager
    {
        [Inject] private DataManager _dataManager;

        private SessionInfo _model;

        public int CountSession => _model.CountSession;
        public bool IsFirstSession => CountSession == 0;

        public override void Initialize()
        {
            _model = _dataManager.Load<SessionInfo>(DataType.Sessions, out var isExists);

            if (!isExists)
            {
                _model.CountSession = 0;
            }
            else
            {
                _model.CountSession++;
                _dataManager.Save(DataType.Sessions, _model);
            }

            Save();
        }

        public override void Dispose()
        {
        }

        private void Save()
        {
            _dataManager.Save(DataType.Sessions, _model);
        }
    }
}