using System.Collections.Generic;
using System.Runtime.InteropServices;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipelines;
using Xunit;

namespace nsimplemessagepump.tests
{
    public class QueryPipeline_tests {
        class MyQuery : Query {
            public string Bar;
        }

        class MyQueryResult : QueryResult {
            public string Baz;
        }

        class MyQueryCtx : IMessageContext  {
            public string Foo;
        }
        
        
        [Fact]
        public void Run() {
            var log = new List<string>();
            var sut = new QueryPipeline(loadContext, processQuery);

            var result = sut.Handle(new MyQuery{Bar = "query"});
            
            Assert.Equal("result", ((MyQueryResult)result.Msg).Baz);
            Assert.Empty(result.Notifications);
            Assert.Equal(new[]{"load-query","process-query","ctx"}, log);


            (IMessageContext Ctx, string Version) loadContext(IMessage msg) {
                log.Add("load-" + ((MyQuery)msg).Bar);
                return (new MyQueryCtx {Foo = "ctx"}, "");
            }

            QueryResult processQuery(IMessage msg, IMessageContext ctx) {
                log.Add("process-" + ((MyQuery)msg).Bar);
                log.Add(((MyQueryCtx)ctx).Foo);
                return new MyQueryResult{Baz = "result"};
            }
        }
    }
}