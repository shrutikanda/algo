using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.MultiAgent
{
    public class ChatEndpoints
    {

        public void Map(WebApplication app)
        {
            app.MapPost("/chat", async (
                ChatRequest request) =>
            {

            });

        }


    }
}
