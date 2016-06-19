using System;
using SeemplestLight.PortableCore.Timing;

namespace SeemplestLight.PortableCore.TestUtility
{
    /// <summary>
    /// This environment provider allows setting a fake DateTime
    /// for test purposes
    /// </summary>
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        private DateTime _fakeDateTime;

        public void SetDateTimeUtc(DateTime dt)
        {
            _fakeDateTime = dt.ToUniversalTime();
        }
        public DateTime GetCurrenDateTimeUtc()
        {
            return _fakeDateTime;
        }
    }
}