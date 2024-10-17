namespace BuildingBlocks.Messaging;

public class SmscSettings
{
    public bool Mock { get; set; }
    public string URL { get; set; }
    public string REQUESTTYPE { get; set; }
    public string USERNAME { get; set; }
    public string PASSWORD { get; set; }
    public string ORIGIN_ADDR { get; set; }
    public string MOBILENO { get; set; }
    public string MESSAGE { get; set; }
    public string TYPE { get; set; }
}