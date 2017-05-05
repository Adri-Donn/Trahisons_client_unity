using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour {

    public GameObject m_itemPrefab;
    public int m_lengthOfList;

    public Transform m_listContent;
    // Use this for initialization
    void Start () {
        //CreateList();
	}
	
	// Update is called once per frame
	void Update () {
		
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

    public void CreateEventLog(string message)
    {
        GameObject item = Instantiate(m_itemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        item.transform.SetParent(m_listContent, false);
        item.GetComponent<Item>().m_text.text = message;

        Transform itemToMove = m_listContent.GetChild(m_listContent.childCount - 1);
        itemToMove.SetAsFirstSibling();
    }

    public void Next()
    {
        Transform firstInstance = m_listContent.GetChild(0);
        firstInstance.SetAsLastSibling();
    }

}
