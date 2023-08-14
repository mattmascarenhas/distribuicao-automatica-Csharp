using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Google.Api.Gax.ResourceNames;
using System.Net.Mail;
using System.Net;


namespace distribuicao_automatica.Utils {
    internal class Notification {
        //toda vez que a distribuição roda, é acrescentado na tabela do big query
        public static void InsertDataBigQuery() {
            //carregar o json com as credenciais
            GoogleCredential credential = GoogleCredential.FromFile("D:\\CSharp\\integration_count\\stable-glass-391813-94cfec6d8191.json");

            //informar o projeto + credenciais
            BigQueryClient client = BigQueryClient.Create("stable-glass-391813", credential);

            // o ID do conjunto de dados e o nome da tabela.
            var datasetId = "bold";
            var tableId = "count_integration";

            // Crie uma referência à tabela.
            var tableReference = client.GetTable(datasetId, tableId);

            // Crie uma lista de linhas para inserir.
            var rows = new List<BigQueryInsertRow>
            {
            new BigQueryInsertRow
            {
                { "id", Guid.NewGuid().ToString()},
                { "integration", "Distribuição Automática" },
                { "updatedAt", DateTime.UtcNow },
            }
        };

            // Insira as linhas na tabela.
            client.InsertRows(datasetId, tableId, rows);

            Console.WriteLine("Dados inseridos com sucesso!");
        }
        //retorna o tamanho da tabela, para saber quantas vezes a distribuição rodou
        public static long GetRowCountFromBigQuery() {
            // Carregar o JSON com as credenciais.
            GoogleCredential credential = GoogleCredential.FromFile("D:\\CSharp\\integration_count\\stable-glass-391813-94cfec6d8191.json");

            // Informar o projeto + credenciais.
            BigQueryClient client = BigQueryClient.Create("stable-glass-391813", credential);

            // O ID do conjunto de dados e o nome da tabela.
            var datasetId = "bold";
            var tableId = "count_integration";

            // Execute a consulta para contar o número de linhas na tabela.
            var sql = $"SELECT COUNT(*) as row_count FROM `{client.ProjectId}.{datasetId}.{tableId}`";
            BigQueryJob job = client.CreateQueryJob(sql, null);

            // Espere a conclusão da consulta.
            job.PollUntilCompleted();

            // Verifique se a consulta foi executada com sucesso e se o resultado está presente.
            var result = job.GetQueryResults().FirstOrDefault();
            if (result != null && result["row_count"] != null) {
                return Convert.ToInt64(result["row_count"]);
            }

            // Caso algo dê errado, retorne -1 ou lance uma exceção, conforme apropriado para sua aplicação.
            return -1;
        }
        //verifica se é para mandar o email
        public static void VerificationToSendEmail() {
            //valor maximo do plano
            long valueMax = 100;
            //valor para enviar a notificação
            long valueToNotify = 71;
            //porcentagem 
            double percentage = (double) valueToNotify / valueMax; 
            long count = GetRowCountFromBigQuery();
            if (count == valueToNotify) {
                SendEmail(percentage);
            }
            Console.WriteLine(count);
            Console.WriteLine(percentage);
        }
        //envia email para o cliente
        public static void SendEmail(double percentage) {
            double _percentage = percentage * 100;
            // Configurações do servidor SMTP
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 25;
            string smtpUsername = "mattmascarenhas7@gmail.com";
            string smtpPassword = "wvkbbwueksgbenep";

            // Configurações do e-mail
            string fromEmail = "mattmascarenhas7@gmail.com";
            string[] toEmails = {
                "matheus.mascarenhas@boldsolution.com.br"
            };
            string subject = "Test Email - Notification";
            string body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            margin: 0;
                            width: 100%;
                            max-width: 100%;
                            padding: 20px;
                            display: flex;
                            justify-content: center;
                            margin-top: 40px;
                        }}
                        .container {{
                            border: 2px solid #000;
                            padding: 10px;
                            border-radius: 10px;
                            width: 40%;
                        }}
                        @media (max-width: 1024px) {{
                            .container {{
                                width: 90%;
                            }}
                        }}
                        .header {{
                            background-color: white;
                            color: black;
                            padding: 10px;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .footer {{
                            background-color: #f0f0f0;
                            border: 2px solid transparent;
                            padding: 10px;
                            border-radius: 10px;
                        }}
                        .header img {{
                            width: 40%;
                            max-width: 100%;
                            display: block;
                            margin: 0 auto;
                        }}
                        @media (max-width: 1024px) {{
                            .header img {{
                                width: 100%;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <img
                                src=""https://gcdnb.pbrd.co/images/FtjMX6pxeAhA.png""
                                alt=""Logo Bold Solution""
                            />
                        </div>
                        <div class=""content"">
                            <p>Prezado(a) [Nome do Cliente],</p>
                            <p>
                                Gostaríamos de informar que você atingiu {_percentage.ToString("0.00")}% do seu plano de
                                assinatura de Distribuição Automática. Lembre-se de que estamos à
                                disposição para qualquer assistência necessária.
                            </p>
                        </div>
                        <div class=""footer"">
                            <p>Atenciosamente, Bold Solution</p>
                        </div>
                    </div>
                </body>
                </html>";

            // Crie uma mensagem de e-mail
            MailMessage message = new MailMessage();
            foreach (string toEmail in toEmails) {
                message.IsBodyHtml = true;
                message.To.Add(toEmail);
            }
            message.From = new MailAddress(fromEmail);
            message.Subject = subject;
            message.Body = body;

            // Crie o cliente SMTP
            SmtpClient smtpClient = new SmtpClient(smtpServer) {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            };

            try {
                // Envie o e-mail
                smtpClient.Send(message);
                Console.WriteLine("E-mail enviado com sucesso!");
            } catch (Exception ex) {
                Console.WriteLine("Erro ao enviar o e-mail: " + ex.Message);
            }
        }

    }
}
