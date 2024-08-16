using System.Net.Sockets;
using System.Text;

/// <summary>
/// Auteur: FauZaPespi
/// Date: 16/08/2024 13:36
/// Detail: Le client du projet fastCalc ce projet est une sorte de protocole pour effectuer des calcul plus rapidement a l'aide d'autre client.
/// </summary>

class Client
{
    private static int nextCalculationId = 1;
    private static Dictionary<int, string> pendingCalculations = new Dictionary<int, string>();

    static void Main(string[] args)
    {
        // Initialise la connexion au serveur sur l'adresse IP et le port spécifiés
        TcpClient client = new TcpClient("127.0.0.1", 8888);
        NetworkStream stream = client.GetStream();

        // Démarre un thread pour recevoir les données du serveur
        Thread receiveThread = new Thread(() => ReceiveData(client));
        receiveThread.Start();

        // Boucle principale pour lire les entrées de l'utilisateur et envoyer des calculs au serveur
        while (true)
        {
            Console.WriteLine("Enter calculation to send to server (e.g., 3+4): ");
            string calculation = Console.ReadLine();

            int calculationId = nextCalculationId++;
            pendingCalculations[calculationId] = calculation;

            // Envoie le calcul au serveur
            SendMessage(client, $"CALC|{calculation}");
        }
    }

    /// <summary>
    /// Réceptionne les données envoyées par le serveur et traite les commandes reçues.
    /// </summary>
    /// <param name="client">L'objet TcpClient utilisé pour la connexion.</param>
    private static void ReceiveData(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        while (true)
        {

            // Décode le message reçu et le traite en fonction de la commande
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            string[] parts = message.Split('|');
            string command = parts[0];

            if (command == "CALC")
            {
                // Reçoit un calcul à effectuer
                int calculationId = int.Parse(parts[1]);
                string calculation = parts[2];
                string result;

                try
                {
                    // Effectue le calcul et envoie le résultat au serveur
                    result = Calculate(calculation);
                    SendMessage(client, $"RESULT|{calculationId}|{result}");
                }
                catch
                {
                    // Envoie un message au serveur si le calcul est trop difficile
                    Console.WriteLine($"Calculation too hard: {calculation}");
                    SendMessage(client, $"TOO_HARD|{calculationId}");
                }
            }
            else if (command == "RESULT")
            {
                // Affiche le résultat du calcul reçu
                int calculationId = int.Parse(parts[1]);
                string result = parts[2];

                if (pendingCalculations.ContainsKey(calculationId))
                {
                    Console.WriteLine($"Result for calculation '{pendingCalculations[calculationId]}': {result}");
                    pendingCalculations.Remove(calculationId);
                }
            }
        }
    }

    /// <summary>
    /// Effectue un calcul basé sur l'expression reçue.
    /// </summary>
    /// <param name="calculation">L'expression de calcul à effectuer.</param>
    /// <returns>Le résultat du calcul sous forme de chaîne.</returns>
    private static string Calculate(string calculation)
    {
        if (calculation == "CalculateComplexSum")
        {
            // Effectue un calcul complexe si l'expression correspond
            return CalculateComplexSum();
        }
        else
        {
            // Utilise un DataTable pour évaluer l'expression mathématique
            var dataTable = new System.Data.DataTable();
            var result = dataTable.Compute(calculation, "");
            return result.ToString();
        }
    }

    /// <summary>
    /// Calcule la somme de la série de l'inverse des factorielles jusqu'à 50000 termes.
    /// </summary>
    /// <returns>Le résultat du calcul complexe sous forme de chaîne.</returns>
    private static string CalculateComplexSum()
    {
        double sum = 0;
        double factorial = 1;

        for (int i = 1; i <= 50000; i++)
        {
            factorial *= i;
            sum += 1 / factorial;
        }

        return sum.ToString();
    }

    /// <summary>
    /// Envoie un message au serveur.
    /// </summary>
    /// <param name="client">L'objet TcpClient utilisé pour la connexion.</param>
    /// <param name="message">Le message à envoyer au serveur.</param>
    private static void SendMessage(TcpClient client, string message)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }
}


// J'ai utiliser ChatGPT pour faire les commentaires.