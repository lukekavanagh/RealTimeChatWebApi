using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PusherRealTimeChat.WebAPI.Models;
using PusherServer;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PusherRealTimeChat.WebAPI.Controllers
{
    [EnableCors("*", "*", "*")]
    public class MessageController : ApiController 
    {
        private static List<ChatMessage> messages = new List<ChatMessage>()
        {
            new ChatMessage
            {
                AuthorTwitterHandle = "Pusher",
                Text = "Hi There! :-)"
            },

            new ChatMessage
            {
                AuthorTwitterHandle = "Pusher",
                Text = "Welcome to your chat app, now fuck off"
            }
        };

        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(
                HttpStatusCode.OK,
                messages);
        }

        public HttpResponseMessage Post(ChatMessage message)
        {
            if (message == null || !ModelState.IsValid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Invalid Input");
            }
            messages.Add(message);

            var pusher = new Pusher(
               "235560",
               "006883c490135c37f8b0",
               "863a20d7dbaf39fcb109",
                    new PusherOptions
                    {
                        Cluster = "MT1 (US-EAST-1)"
                    });

            pusher.Trigger(
                channelName: "messages",
                eventName: "new_message",
                data: new
                {
                    AuthorTwitterHandle = message.AuthorTwitterHandle,
                    Text = message.Text
                });
        
            return Request.CreateResponse(HttpStatusCode.Created);
        }


    }
}