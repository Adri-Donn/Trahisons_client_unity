using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts;
using System.Net.Sockets;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Threading;

public class NetworkManager : MonoBehaviour {

    TcpClient clientSocket = new TcpClient();
    NetworkStream serverStream;

    public static List<Message> fileAttente = new List<Message>();

    public string nameOfActualRoom;

    public static bool canMakeAction = false;
    static List<User> room = new List<User>();
    public int orderNumberActualPlayer = 9999;

    public GameObject TourLogs;
    Scrollbar tourLogs_SB;

    public GameObject EventsLogs;
    Scrollbar eventsLogs_SB;

    public GameObject amountOfMoneyGOB;
    GUIText amountOfMoneyGUIText;

    public GameObject Nombre_Ambassadeurs;
    GUIText Nombre_Ambassadeurs_GT;
    public GameObject Nombre_Comptesses;
    GUIText Nombre_Comptesses_GT;
    public GameObject Nombre_Capitaines;
    GUIText Nombre_Capitaines_GT;
    public GameObject Nombre_Tueurs;
    GUIText Nombre_Tueurs_GT;
    public GameObject Nombre_Duchesses;
    GUIText Nombre_Duchesses_GT;
    public GameObject Nombre_Inquisiteurs;
    GUIText Nombre_Inquisiteurs_GT;
    
    public GameObject Dock_Ambassador_Card;
    Button Dock_Ambassador_Card_SR;
    public GameObject Dock_Capitain_Card;
    Button Dock_Capitain_Card_SR;
    public GameObject Dock_Comptess_Card;
    Button Dock_Comptess_Card_SR;
    public GameObject Dock_Duchess_Card;
    Button Dock_Duchess_Card_SR;
    public GameObject Dock_Inquisitor_Card;
    Button Dock_Inquisitor_Card_SR;
    public GameObject Dock_Killer_Card;
    Button Dock_Killer_Card_SR;

    public GameObject Action_Revenue;
    Button Action_Revenue_SR;

    public GameObject Action_SocialHelp;
    Button Action_SocialHelp_SR;

    public GameObject CarteVierge;
    SpriteRenderer CarteVierge_SR;

    public GameObject Action_Challenge;
    Button Action_Challenge_SR;

    public GameObject WaitingRoom;
    Image WaitingRoom_I;

    public void Init()
    {
        TourLogs = GameObject.Find("TourLogs");
        tourLogs_SB = TourLogs.GetComponent<Scrollbar>();

        EventsLogs = GameObject.Find("EventsLogs");
        eventsLogs_SB = EventsLogs.GetComponent<Scrollbar>();

        amountOfMoneyGOB = GameObject.Find("AmountOfMoney");
        amountOfMoneyGUIText = amountOfMoneyGOB.GetComponent<GUIText>();

        Nombre_Ambassadeurs = GameObject.Find("Nombre_Ambassadeurs");
        Nombre_Ambassadeurs_GT = Nombre_Ambassadeurs.GetComponent<GUIText>();
        Nombre_Comptesses = GameObject.Find("Nombre_Comptesses");
        Nombre_Comptesses_GT = Nombre_Comptesses.GetComponent<GUIText>();
        Nombre_Capitaines = GameObject.Find("Nombre_Capitaines");
        Nombre_Capitaines_GT = Nombre_Capitaines.GetComponent<GUIText>();
        Nombre_Tueurs = GameObject.Find("Nombre_Tueurs");
        Nombre_Tueurs_GT = Nombre_Tueurs.GetComponent<GUIText>();
        Nombre_Duchesses = GameObject.Find("Nombre_Duchesses");
        Nombre_Duchesses_GT = Nombre_Duchesses.GetComponent<GUIText>();
        Nombre_Inquisiteurs = GameObject.Find("Nombre_Inquisiteurs");
        Nombre_Inquisiteurs_GT = Nombre_Inquisiteurs.GetComponent<GUIText>();

        Dock_Ambassador_Card = GameObject.Find("Dock-Ambassador");
        Dock_Ambassador_Card_SR = Dock_Ambassador_Card.GetComponent<Button>();
        Dock_Capitain_Card = GameObject.Find("Dock-Capitain");
        Dock_Capitain_Card_SR = Dock_Capitain_Card.GetComponent<Button>();
        Dock_Comptess_Card = GameObject.Find("Dock-Comptess");
        Dock_Comptess_Card_SR = Dock_Comptess_Card.GetComponent<Button>();
        Dock_Duchess_Card = GameObject.Find("Dock-Duchess");
        Dock_Duchess_Card_SR = Dock_Duchess_Card.GetComponent<Button>();
        Dock_Inquisitor_Card = GameObject.Find("Dock-Inquisitor");
        Dock_Inquisitor_Card_SR = Dock_Inquisitor_Card.GetComponent<Button>();
        Dock_Killer_Card = GameObject.Find("Dock-Killer");
        Dock_Killer_Card_SR = Dock_Killer_Card.GetComponent<Button>();

        Action_Revenue = GameObject.Find("Revenue");
        Action_Revenue_SR = Action_Revenue.GetComponent<Button>();

        Action_SocialHelp = GameObject.Find("SocialHelp");
        Action_SocialHelp_SR = Action_SocialHelp.GetComponent<Button>();

        Dock_Ambassador_Card_SR.onClick.AddListener(ActionAmbassador);
        Dock_Capitain_Card_SR.onClick.AddListener(ActionCapitain);
        Dock_Comptess_Card_SR.onClick.AddListener(ActionComptess);
        Dock_Duchess_Card_SR.onClick.AddListener(ActionDuchess);
        Dock_Inquisitor_Card_SR.onClick.AddListener(ActionInquisitor);
        Dock_Killer_Card_SR.onClick.AddListener(ActionKiller);

        Action_Revenue_SR.onClick.AddListener(ActionRevenue);

        Action_SocialHelp_SR.onClick.AddListener(ActionSocialHelp);

        CarteVierge = GameObject.Find("CarteVierge");
        CarteVierge_SR = CarteVierge.GetComponent<SpriteRenderer>();

        Action_Challenge = GameObject.Find("Challenge");
        Action_Challenge_SR = Action_Challenge.GetComponent<Button>();
        Action_Challenge_SR.onClick.AddListener(ActionChallenge);

        WaitingRoom = GameObject.Find("WaitingRoom");
        WaitingRoom_I = WaitingRoom.GetComponent<Image>();
    }

    // Use this for initialization
    void Start()
    {
        Init();
        CanPlay(true);
        SetActualRoomName("Biere <3");

        string[] ServerAddress = PlayerPrefs.GetString("serverIp").Split(':');
        string ServerIp = ServerAddress[0];
        int ServerPort = Convert.ToInt32(ServerAddress[1]);

        Debug.Log("Client Started");
        clientSocket.Connect(ServerIp, ServerPort);
        Debug.Log("Client Socket Program - Server Connected ...");

        var thread = new Thread(() => SocketMessage());
        thread.Start();
    }

    private void Send(TcpClient client, Message message)
    {
        try
        {
            Byte[] broadcastBytes = null;

            var xmlSerializer = new XmlSerializer(typeof(Message));
            NetworkStream networkStream = client.GetStream();
        
            xmlSerializer.Serialize(networkStream, message);

            broadcastBytes = Encoding.UTF8.GetBytes(xmlSerializer.ToString());

            networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
            networkStream.Flush();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void SocketMessage()
    {
        while (clientSocket.Connected)
        {
            try
            {
                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                NetworkStream networkStream = clientSocket.GetStream();
                int n = networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                if (n == 0)
                    break;
                string dataFromClient = Encoding.UTF8.GetString(bytesFrom);
                
                var deserialize = new XmlSerializer(typeof(Message));
                Message message = (Message)deserialize.Deserialize(new StringReader(cleanDataReceived(dataFromClient)));

                fileAttente.Add(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Data);
                Console.WriteLine(e.InnerException);
            }
            finally
            {

            }
            
        }
    }

    void SendMessageToServer(Message message)
    {
        Send(clientSocket, message);
    }

    public string cleanDataReceived(string data)
    {
        int fc = 0;
        int lc = 0;
        int pos = 0;

        foreach (char c in data)
        {
            if (c == '<' && fc > pos)
            {
                fc = pos;
            }
            else if (c == '>' && lc < pos)
            {
                lc = pos;
            }

            pos++;
        }

        return data.Substring(fc, lc - fc + 1);
    }

    public void GetSRVVERS()
    {
        SendMessageToServer(new Message(EnumTypeMSG.GET, EnumTypeACTIONS.SRVVERS, null, null, null, null));
    }
    
    private void AnalyseMessage(Message message)
    {
        Debug.Log("Analyse d'un message en cour!" + message.typeMSG + " : " + message.typeAction + " : " + message.a + " : " + message.b + " : " + message.c + " : " + message.d);
        switch (message.typeMSG)
        {
            case EnumTypeMSG.SET:
                switch(message.typeAction)
                {
                    case EnumTypeACTIONS.ROOM:
                        SetActualRoomName(message.a);
                        break;

                    case EnumTypeACTIONS.MONEY:
                        room[Convert.ToInt32(message.a)].Money = Convert.ToInt32(message.b);
                        if(orderNumberActualPlayer == 9999)
                        {
                            if(Convert.ToInt32(message.a) == orderNumberActualPlayer)
                            {
                                amountOfMoneyGUIText.text = message.b;
                            }
                            else
                            {
                                // mettre à jour la carte du joueur => money joueur
                                room[Convert.ToInt32(message.a)].Money = Convert.ToInt32(message.b);
                            }
                        }
                        InsertIntoLogger("Le joueur " + room[Convert.ToInt32(message.a)].Name + " a " + message.b + " pièces.");
                        break;

                    case EnumTypeACTIONS.PLAYER:

                        room[Convert.ToInt32(message.a)].Name = message.b;

                        // mettre la carte du joueur à jour
                        // message.b = order number
                        // message.c = playerName
                        break;

                    case EnumTypeACTIONS.NAMEOFAPLAYER:
                        room[Convert.ToInt32(message.a)].Name = message.b;
                        // mettre la carte du joueur à jour
                        // message.b = order number
                        // message.c = playerName
                        break;

                    case EnumTypeACTIONS.NUMBEROFPLAYERS:
                        while (room.Count < Convert.ToInt32(message.a)) {
                            room.Add(new User());
                        }
                        // message.b = number of players
                        InsertIntoLogger("Il y a maintenant " + message.a + " joueurs dans la salle.");
                        break;

                    case EnumTypeACTIONS.NUMBER_AMBASSADOR:
                        Nombre_Ambassadeurs_GT.text = message.a;
                        InsertIntoLogger("Il reste " + message.a + " ambassadeurs.");
                        break;

                    case EnumTypeACTIONS.NUMBER_COMPTESS:
                        Nombre_Comptesses_GT.text = message.a;
                        InsertIntoLogger("Il reste " + message.a + " comptesses.");
                        break;

                    case EnumTypeACTIONS.NUMBER_CAPITAIN:
                        Nombre_Capitaines_GT.text = message.a;
                        InsertIntoLogger("Il reste " + message.a + " capitaines.");
                        break;

                    case EnumTypeACTIONS.NUMBER_DUCHESS:
                        Nombre_Duchesses_GT.text = message.a;
                        InsertIntoLogger("Il reste " + message.a + " duchesses.");
                        break;

                    case EnumTypeACTIONS.NUMBER_INQUISITOR:
                        Nombre_Inquisiteurs_GT.text = message.a;
                        InsertIntoLogger("Il reste " + message.a + " inquisitors.");
                        break;

                    case EnumTypeACTIONS.NUMBER_KILLER:
                        Nombre_Tueurs_GT.text = message.a;
                        InsertIntoLogger("Il reste " + message.a + " tueurs.");
                        break;
                }
                break;

            case EnumTypeMSG.ANNONCE:
                switch(message.typeAction)
                {
                    case EnumTypeACTIONS.NEWPLAYER:
                        // message.a = name of the player

                        InsertIntoLogger("Nouveau joueur : " + message.a + ".");
                        break;

                    case EnumTypeACTIONS.SOCIALHELP:
                        if(message.a == "SUCESS")
                        {
                            // azertyui
                        }
                        else
                        {
                            // Afficher que le joueur demande une aide
                        }
                        // message.a = order number of player doing. SUCESS
                        break;

                    case EnumTypeACTIONS.AMBASSADORSELECTINGCARDS:
                        // message.a = order number target player. SUCCESS
                        break;

                    case EnumTypeACTIONS.CAPITAINSTEALINGMONEY:
                        // message.a = order number of player doing. or SUCCESS
                        break;

                    case EnumTypeACTIONS.DUCHESSTAKINGINBANK:
                        // message.a = order number of player doing.  SUCCESS
                        break;

                    case EnumTypeACTIONS.INQUISITORCONSULTING:
                        // message.a = order number of player doing.  SUCESS or 
                        break;

                    case EnumTypeACTIONS.INQUISITORSELECTINGCARD:
                        // message.a = order number of player doing. SUCCESS
                        break;

                    case EnumTypeACTIONS.KILLERKILLING:
                        // message.a = order number of player doing. SUCCESS
                        break;

                    case EnumTypeACTIONS.MURDER:
                        // message.a = order number target player
                        break;

                    case EnumTypeACTIONS.MONEYERROR:

                        break;

                    case EnumTypeACTIONS.CHALLENGE:

                        break;

                    case EnumTypeACTIONS.NOCHALLENGE:

                        break;

                    case EnumTypeACTIONS.WINNER:
                        // message.a = order number target player
                        break;

                    case EnumTypeACTIONS.SECONDCHALLENGE:
                        switch (message.a)
                        {
                            case "FAIL":

                                break;

                            case "SUCCESS":

                                break;
                        }
                        break;
                        
                    case EnumTypeACTIONS.LOOSEONECARD:

                        break;

                    case EnumTypeACTIONS.KILLONECARD:
                        // message.a = player who must kill one card => print if number of actual player is different.
                        break;

                    case EnumTypeACTIONS.KILLED:
                        // number order of player dawn
                        break;
                }
                break;

            case EnumTypeMSG.ORDER:
                switch(message.typeAction)
                {
                    case EnumTypeACTIONS.MUSTKILLONECARD:

                        break;

                    case EnumTypeACTIONS.WAIT:
                        CanPlay(false);
                        break;

                    case EnumTypeACTIONS.YOUR:
                        CanPlay(true);
                        break;
                }
                break;

            case EnumTypeMSG.GET:

                break;

            case EnumTypeMSG.ANSWER:
                switch(message.typeAction)
                {
                    case EnumTypeACTIONS.SRVVERS:
                        Debug.Log(message.a);
                        break;

                    case EnumTypeACTIONS.MINCLTVERS:

                        break;
                }
                break;

            case EnumTypeMSG.SELECTION:
                switch (message.typeAction)
                {
                    case EnumTypeACTIONS.CARD:
                        // message.a =  card
                        break;

                    case EnumTypeACTIONS.CARDS:
                        // message.a = first card
                        // message.b = second card
                        break;
                }
                break;

            case EnumTypeMSG.CONSULT:
                switch (message.typeAction)
                {
                    case EnumTypeACTIONS.CARD:
                        // message.a =  target player
                        // message.b = card
                        break;
                }
                break;

            case EnumTypeMSG.NEXTPLAYER:
                // message.a == nextplayer
                break;
        }

        // Mettre à jour écran
    }

    public void ResetGame()
    {
        CarteVierge_SR.sprite = Resources.Load("CarteVierge", typeof(Sprite)) as Sprite;
    }

    public void SetActualRoomName(string actualRoom)
    {
        nameOfActualRoom = actualRoom;
        if (actualRoom == "waitingRoom")
        {
            Debug.Log("Room : WaitingRoom");
            WaitingRoom_I.enabled = true;
            //salle d'attente
        }
        else
        {
            Debug.Log("Room : " + actualRoom);
            WaitingRoom_I.enabled = false;
            ResetGame();
        }
        InsertIntoLogger("Salle rejointe.");
    }

    public void InsertIntoLogger(string message)
    {
        // ici
    }

    // Update is called once per frame
    void Update () {
        InsertIntoLogger("TEst");

        if(fileAttente.Count() > 0)
        {
            AnalyseMessage(fileAttente[0]);
            fileAttente.RemoveAt(0);
        }

        
    }


    public void CanPlay(bool canPlay)
    {
        Debug.Log("Hidding action cards");

        canMakeAction = canPlay;

        Dock_Ambassador_Card_SR.enabled = canPlay;
        Dock_Capitain_Card_SR.enabled = canPlay;
        Dock_Comptess_Card_SR.enabled = canPlay;
        Dock_Duchess_Card_SR.enabled = canPlay;
        Dock_Inquisitor_Card_SR.enabled = canPlay;
        Dock_Killer_Card_SR.enabled = canPlay;

        Action_Revenue_SR.enabled = canPlay;
        Action_Challenge_SR.enabled = canPlay;
        Action_SocialHelp_SR.enabled = canPlay;
    }

    public void ActionAmbassador()
    {
        Debug.Log("Ambassador");
        SetSprite(CarteVierge_SR, "Ambassadeur");
    }

    public void ActionCapitain()
    {
        Debug.Log("Capitain");
        SetSprite(CarteVierge_SR, "Capitaine");
    }

    public void ActionComptess()
    {
        Debug.Log("Comptess");
        SetSprite(CarteVierge_SR, "Comptesse");
    }

    public void ActionDuchess()
    {
        Debug.Log("Duchess");
        SetSprite(CarteVierge_SR, "Duchesse");
    }

    public void ActionInquisitor()
    {
        Debug.Log("Inquisitor");
        SetSprite(CarteVierge_SR, "Inquisiteur");
    }

    public void ActionKiller()
    {
        Debug.Log("Killer");
        SetSprite(CarteVierge_SR, "Tueur");
    }

    public void ActionRevenue()
    {
        Debug.Log("Revenue");
        SetSprite(CarteVierge_SR, "Revenue");
    }

    public void ActionSocialHelp()
    {
        Debug.Log("SocialHelp");
        SetSprite(CarteVierge_SR, "StrangerHelp 1");
    }

    public void ActionChallenge()
    {
        Debug.Log("Challenge!");
        SetSprite(CarteVierge_SR, "poing");
    }

    public void SetSprite(SpriteRenderer spriteRenderer, string spriteName)
    {
        spriteRenderer.sprite = Resources.LoadAll<Sprite>("").Where(a => a.name == spriteName).First();
    }
}