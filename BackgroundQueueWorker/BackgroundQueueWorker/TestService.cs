using System;

namespace BackgroundQueueWorker
{
    public class TestService
    {
        private Guid _myId;

        public TestService()
        {
            _myId = Guid.NewGuid();
        }

        public string GetId() => _myId.ToString("N");
    }
}
