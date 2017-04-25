public class Message
{
    public Message() { }
    public Message(EnumTypeMSG typeMSG, EnumTypeACTIONS enumTypeActions, string a, string b, string c, string d)
    {
        this.typeMSG = typeMSG;
        this.typeAction = enumTypeActions;
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }
    public EnumTypeMSG typeMSG { get; set; }
    public EnumTypeACTIONS typeAction { get; set; }
    public string a { get; set; }
    public string b { get; set; }
    public string c { get; set; }
    public string d { get; set; }
}

public enum EnumTypeMSG
{
    ACTION,
    ORDER,
    ANNONCE,
    PROPOSITION,
    SELECTION,
    CONSULT,
    GET,
    SET,
    ANSWER,
    NEXTPLAYER,
    RESET
}

public enum EnumTypeACTIONS
{
    ROOM,
    REVENUE,
    NEWPLAYER,
    MONEY,
    MONEYERROR,
    SOCIALHELP,
    POWER,
    MURDER,
    CHALLENGE,
    SECONDCHALLENGE,
    AMBASSADORSELECTINGCARDS,
    RETRIEVINGCARDSAMBASSADOR,
    CAPITAINSTEALINGMONEY,
    DUCHESSTAKINGINBANK,
    INQUISITORCONSULTING,
    INQUISITORSELECTINGCARD,
    RETRIEVINGCARDINQUISITOR,
    KILLERKILLING,
    KILLONECARD,
    MUSTKILLONECARD,
    KILLCARD,
    KILLED,
    WAIT,
    SRVVERS,
    MINCLTVERS,
    NAME,
    NAMEOFAPLAYER,
    NUMBER_AMBASSADOR,
    NUMBER_COMPTESS,
    NUMBER_CAPITAIN,
    NUMBER_DUCHESS,
    NUMBER_INQUISITOR,
    NUMBER_KILLER,
    CARD,
    CARDS,
    NOCHALLENGE,
    WINNER,
    NUMBEROFPLAYERS,
    PLAYER,
    NEXTPLAYER,
    YOUR,
    LOOSEONECARD
}