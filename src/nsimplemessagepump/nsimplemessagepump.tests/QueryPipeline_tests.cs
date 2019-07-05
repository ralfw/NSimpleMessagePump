using System.Collections.Generic;
using System.Runtime.InteropServices;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipelines;
using Xunit;

namespace nsimplemessagepump.tests
{
    public class QueryPipeline_tests {
        class MyQuery : Query {
            public string Prefix;
        }

        class MyQueryResult : QueryResult {
            public string Value;
        }

        class MyQueryCtx : IMessageContext  {
            public string Value;
        }
        
        
        [Fact]
        public void Run() {
            var sut = new QueryPipeline(loadContext, processQuery);

            var result = sut.Handle(new MyQuery{Prefix = ":"});
            
            Assert.Equal(":foo", ((MyQueryResult)result.Msg).Value);
            Assert.Empty(result.Notifications);


            (IMessageContext Ctx, string Version) loadContext(IMessage msg) {
                return (new MyQueryCtx {Value = "foo"}, "");
            }

            QueryResult processQuery(IMessage msg, IMessageContext ctx) {
                return new MyQueryResult{Value = (msg as MyQuery).Prefix + (ctx as MyQueryCtx).Value};
            }
        }
    }
}