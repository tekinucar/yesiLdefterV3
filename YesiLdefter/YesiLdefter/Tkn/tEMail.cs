using OpenPop.Mime;
using OpenPop.Pop3;
using System.Collections.Generic;


namespace Tkn_EMail
{
    class tEMail
    {
        // https://sourceforge.net/projects/hpop/

        /// <summary>
        /// Example showing:
        ///  - how to fetch all messages from a POP3 server
        /// </summary>
        /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
        /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
        /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
        /// <param name="username">Username of the user on the server</param>
        /// <param name="password">Password of the user on the server</param>
        /// <returns>All Messages on the POP3 server</returns>

        /// <summary> 
        /// Örnek gösterme: 
        /// - bir POP3 sunucusundaki tüm iletileri nasıl alabilirim 
        /// </ summary> 
        /// <param name = "ana makine adı"> Sunucu adı. Örneğin: pop3.live.com </ param> 
        /// <param name = "port"> Bağlanmak için ana makine portu. Normalde: 110 düz POP3 için, 995 SSL POP3 için </ param> 
        /// <param name = "useSsl"> Sunucuya bağlanmak için SSL kullanılıp kullanılmayacağı </ param> 
        /// <param name = "kullanıcı adı" > Sunucudaki kullanıcının parolası</ param>
        /// <returns> POP3 sunucusundaki tüm İletiler</ returns> 

        public static List<Message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password)
        {
            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect(hostname, port, useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(username, password);

                // Get the number of messages in the inbox
                int messageCount = client.GetMessageCount();

                // We want to download all messages
                List<Message> allMessages = new List<Message>(messageCount);

                // Messages are numbered in the interval: [1, messageCount]
                // Ergo: message numbers are 1-based.
                // Most servers give the latest message the highest number
                for (int i = messageCount; i > 0; i--)
                {
                    allMessages.Add(client.GetMessage(i));
                }

                // Now return the fetched messages
                return allMessages;
            }
        }

        /*
         * Kodun devamı yukarıdaki adres ordan indirmen gerekiyor

        Basic
            Download all email from server
            Find specific parts of an email (text, html, xml)
            Delete an email on the server
            Save and load a message to/from the filesystem
            Checking headers before downloading full message
            Extracting embedded images from a HTML email
        Medium
            Download "unread" email only
            Delete message using the Message ID
            Visit the whole message hierarchy
            Changing where logging goes
        Advanced
            Override SSL Certificate check
            Change the mapping from character sets to encodings

        */
    }
}