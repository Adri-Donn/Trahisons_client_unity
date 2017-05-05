using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollPlayersManager : MonoBehaviour {

    public GameObject m_itemPrefab;
    public int m_lengthOfList;

    public Transform m_listContent;
    // Use this for initialization
    void Start()
    {
        //CreateList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    public void CreateList()
    {
        for(int i =0; i< m_lengthOfList; i++)
        {
            GameObject item = Instantiate(m_itemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            item.transform.SetParent(m_listContent, false);
            item.GetComponent<Item>().m_text.text = "item " + i;
        }
    }
    */

    public void CreatePlayer(string PlayerName)
    {
        GameObject player = Instantiate(m_itemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        player.transform.SetParent(m_listContent, false);

        player.GetComponent<PlayerInScroll>().PlayerName.text = PlayerName;
        player.GetComponent<PlayerInScroll>().PlayerMoney.text = "2";
        player.GetComponent<PlayerInScroll>().PlayerCard.text = "2";
    }

    public void Next()
    {
        Transform firstInstance = m_listContent.GetChild(0);
        firstInstance.SetAsLastSibling();
    }
}
