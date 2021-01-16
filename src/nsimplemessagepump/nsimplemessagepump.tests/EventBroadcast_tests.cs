using System;
using System.Collections.Generic;
using nsimpleeventstore.contract;
using Xunit;

namespace nsimplemessagepump.tests
{
    public class EventBroadcast_tests
    {
        [Fact]
        public void Subscribers_are_notified_just_once_even_if_subscribed_multiple_times()
        {
            _counters = new int[2];
            var sut = new EventBroadcast();
            sut.Subscribe(Subscribe1);
            sut.Subscribe(Subscribe1);
            sut.Subscribe(Subscribe2);
            sut.Subscribe(Subscribe2);
            sut.Subscribe(Subscribe2);
            
            sut.Update(new Event[0], null);
            
            Assert.Equal(new[]{1,1}, _counters);
        }


        private int[] _counters;

        void Subscribe1(IEvent[] events, EventId lastEventId)
        {
            _counters[0]++;
        }
        
        
        void Subscribe2(IEvent[] events, EventId lastEventId)
        {
            _counters[1]++;
        }
    }
}