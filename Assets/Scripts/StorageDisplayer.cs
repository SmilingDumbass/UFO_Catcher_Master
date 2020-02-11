using UnityEngine;
using UnityEngine.UI;

public class StorageDisplayer : MonoBehaviour
{
    private const int slotLength = 4;
    private Text[] text = new Text[Player.total];
    private string prefix;

    public Texture soldOut;
    public Texture[] textures;
    private RawImage[] slot = new RawImage[slotLength];

    private Text money;

    public Player player;
    public Store store;

    private Text my_cost;

    void Start()
    {
        Transform rightColumn = transform.Find("RightColumn");
        Transform leftColumn = transform.Find("LeftColumn");

        for(int i = 0; i < text.Length; i++)
        {
            text[i] = leftColumn.Find("Text (" + i + ")").GetComponent<Text>();
        }
        prefix = text[0].text;

        for (int i = 0; i < slot.Length; i++)
        {
            slot[i] = rightColumn.Find("Slot (" + i + ")").GetComponent<RawImage>();
        }
        money = rightColumn.Find("Money").GetComponent<Text>();
        my_cost = transform.Find("LeftColumn2").Find("Cost").GetComponent<Text>();
    }
    
    void Update()
    {
        for(int i = 0; i < text.Length; i++)
        {
            text[i].text = prefix + player.GetItem(i);
        }
        for(int i = 0; i < slot.Length; i++)
        {
            int type = store.GetItem(i);
            if (type == -1)
            {
                slot[i].texture = soldOut;
            }
            else
            {
                slot[i].texture = textures[type];
            }
        }
        money.text = player.GetCoin().ToString();
        my_cost.text = player.cost.ToString();
    }
}
