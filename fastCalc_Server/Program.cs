using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

/// <summary>
/// Auteur: FauZaPespi
/// Date: 16/08/2024 13:36
/// Detail: Le Server du projet fastCalc ce projet est une sorte de protocole pour effectuer des calcul plus rapidement a l'aide d'autre client.
/// </summary>

class Server
{
    private static List<TcpClient> clients = new List<TcpClient>();
    private static Dictionary<int, (int ClientId, string Calculation)> pendingCalculations = new Dictionary<int, (int ClientId, string Calculation)>();
    private static string resultFilePath = "result.json";
    private static int nextCalculationId = 1;
    private static object lockObj = new object();

    static void Main(string[] args)
    {
        // Vérifie si le fichier de résultats existe déjà, sinon le crée avec un tableau JSON vide
        if (!File.Exists(resultFilePath))
        {
            File.WriteAllText(resultFilePath, "[]");
        }

        // Initialise le serveur et commence à accepter les connexions des clients
        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Console.WriteLine("Server started...");

        // Boucle principale pour accepter les clients entrants
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            lock (lockObj)
            {
                clients.Add(client);
            }
            int clientId = clients.Count;
            Console.WriteLine($"Client {clientId} connected");
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    /// <summary>
    /// Gère la communication avec un client spécifique.
    /// </summary>
    /// <param name="obj">Le client à gérer, passé sous forme d'objet.</param>
    private static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        int clientId = clients.IndexOf(client) + 1;

        try
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // Vérifie si le client s'est déconnecté
                    if (bytesRead == 0)
                    {
                        Console.WriteLine($"Client {clientId} disconnected.");
                        break;
                    }


                    // Traite le message reçu du client
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    string[] parts = message.Split('|');
                    string command = parts[0];

                    // Traitement des commandes reçues du client
                    if (command == "CALC")
                    {
                        string calculation = parts[1];
                        int calculationId = nextCalculationId++;

                        if (IsCalculationAlreadyDone(calculation, out string result))
                        {
                            Console.WriteLine($"Calculation {calculationId} already done. Sending result to Client {clientId}.");
                            SendMessage(client, $"RESULT|{calculationId}|{result}");
                        }
                        else
                        {
                            lock (lockObj)
                            {
                                pendingCalculations.Add(calculationId, (clientId, calculation));
                            }
                            SendCalculationToOtherClient(clientId, calculationId, calculation);
                        }
                    }
                    else if (command == "RESULT")
                    {
                        int calculationId = int.Parse(parts[1]);
                        string result = parts[2];

                        lock (lockObj)
                        {
                            var originalRequest = pendingCalculations[calculationId];
                            SaveResult(calculationId, originalRequest.Calculation, result);
                            SendMessage(clients[originalRequest.ClientId - 1], $"RESULT|{calculationId}|{result}");
                            pendingCalculations.Remove(calculationId);
                        }
                    }
                    else if (command == "TOO_HARD")
                    {
                        int calculationId = int.Parse(parts[1]);
                        lock (lockObj)
                        {
                            var originalRequest = pendingCalculations[calculationId];
                            SendCalculationToOtherClient(clientId, calculationId, originalRequest.Calculation);
                        }
                    }
                }
                catch (IOException ioEx)
                {
                    // Gère les exceptions liées à la connexion réseau
                    Console.WriteLine($"IOException: {ioEx.Message}");
                    Console.WriteLine($"Client {clientId} disconnected.");
                    break;
                }
                catch (Exception ex)
                {
                    // Gère les exceptions générales
                    Console.WriteLine($"Exception: {ex.Message}");
                    break;
                }
            }
        }
        finally
        {
            // Assure que le client est retiré de la liste même en cas d'exception
            lock (lockObj)
            {
                clients.Remove(client);
            }
        }
    }


    /// <summary>
    /// Envoie un calcul à un autre client.
    /// </summary>
    /// <param name="originalClientId">L'identifiant du client d'origine qui a demandé le calcul.</param>
    /// <param name="calculationId">L'identifiant du calcul à envoyer.</param>
    /// <param name="calculation">L'expression de calcul à envoyer.</param>
    private static void SendCalculationToOtherClient(int originalClientId, int calculationId, string calculation)
    {
        lock (lockObj)
        {
            foreach (var client in clients)
            {
                int clientId = clients.IndexOf(client) + 1;
                if (clientId != originalClientId)
                {
                    SendMessage(client, $"CALC|{calculationId}|{calculation}");
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Envoie un message à un client spécifique.
    /// </summary>
    /// <param name="client">Le client à qui envoyer le message.</param>
    /// <param name="message">Le message à envoyer.</param>
    private static void SendMessage(TcpClient client, string message)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }


    /// <summary>
    /// Vérifie si un calcul a déjà été effectué et récupère le résultat si disponible.
    /// </summary>
    /// <param name="calculation">L'expression de calcul à vérifier.</param>
    /// <param name="result">Le résultat du calcul si disponible.</param>
    /// <returns>Vrai si le calcul a déjà été effectué, sinon faux.</returns>
    private static bool IsCalculationAlreadyDone(string calculation, out string result)
    {
        var existingResults = JsonConvert.DeserializeObject<List<Result>>(File.ReadAllText(resultFilePath));
        var existingResult = existingResults?.FirstOrDefault(r => r.Calculation == calculation);

        if (existingResult != null)
        {
            result = existingResult.ResultValue;
            return true;
        }

        result = null;
        return false;
    }


    /// <summary>
    /// Sauvegarde le résultat d'un calcul dans un fichier JSON.
    /// </summary>
    /// <param name="calculationId">L'identifiant du calcul.</param>
    /// <param name="calculation">L'expression de calcul.</param>
    /// <param name="result">Le résultat du calcul.</param>
    private static void SaveResult(int calculationId, string calculation, string result)
    {
        var existingResults = JsonConvert.DeserializeObject<List<Result>>(File.ReadAllText(resultFilePath)) ?? new List<Result>();
        existingResults.Add(new Result { Id = calculationId, Calculation = calculation, ResultValue = result });
        File.WriteAllText(resultFilePath, JsonConvert.SerializeObject(existingResults));
        Console.WriteLine($"Result saved for calculation {calculationId}: {result}");
    }

    /// <summary>
    /// Représente un résultat de calcul stocké dans le fichier JSON.
    /// </summary>
    private class Result
    {
        public int Id { get; set; }
        public string Calculation { get; set; }
        public string ResultValue { get; set; }

        public Result() { } // Constructeur par défaut nécessaire pour la sérialisation
    }
}

// J'ai utiliser ChatGPT pour faire les commentaires.