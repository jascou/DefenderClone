using System.Collections;
using UnityEngine;

public class TriggerDispatcher : MonoBehaviour {
	public bool trackTriggerEnter;
	public bool trackTriggerExit;
	public bool trackTriggerStay;
	public string triggerEnterEvent;
	public string triggerExitEvent;
	public string triggerStayEvent;
	public string dataParam;
	/*
	A listener for dispatching various trigger events to global audience
	 */
	void OnTriggerEnter2D(Collider2D other){
		if(!trackTriggerEnter)return;
		if (dataParam.Length > 0) {
			Hashtable data=new Hashtable(1);
			data.Add("name",dataParam);
			data.Add("who",other.gameObject);
			NotificationCenter.Notification note = new NotificationCenter.Notification (this, triggerEnterEvent, data);
			NotificationCenter.DefaultCenter.PostNotification(note);
		} else {
			NotificationCenter.DefaultCenter.PostNotification(this, triggerEnterEvent);
		} 
    }
	void OnTriggerExit2D(Collider2D other)
    {
        if(!trackTriggerExit)return;
		if (dataParam.Length > 0) {
			Hashtable data=new Hashtable(1);
			data.Add("name",dataParam);
			data.Add("who",other.gameObject);
			NotificationCenter.Notification note = new NotificationCenter.Notification (this, triggerExitEvent, data);
			NotificationCenter.DefaultCenter.PostNotification(note);
		} else {
			NotificationCenter.DefaultCenter.PostNotification(this, triggerExitEvent);
		} 
    }
	void OnTriggerStay2D(Collider2D other)
    {
        if(!trackTriggerStay)return;
		if (dataParam.Length > 0) {
			Hashtable data=new Hashtable(1);
			data.Add("name",dataParam);
			data.Add("who",other.gameObject);
			NotificationCenter.Notification note = new NotificationCenter.Notification (this, triggerStayEvent, data);
			NotificationCenter.DefaultCenter.PostNotification(note);
		} else {
			NotificationCenter.DefaultCenter.PostNotification(this, triggerStayEvent);
		} 
    }
}
