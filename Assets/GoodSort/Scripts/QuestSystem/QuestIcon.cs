using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestIcon : MonoBehaviour
{
    [SerializeField] GameObject _canDoneQuestIcon;
    [SerializeField] GameObject _canStartQuestIcon;
    [SerializeField] GameObject _notMetReuirementDoneQuestIcon;
    [SerializeField] GameObject _notMetRequirementStartQuestIcon;

    public void SetState(QuestState state, bool startPoint, bool endPoint)
    {
        _canDoneQuestIcon.SetActive(false);
        _canStartQuestIcon.SetActive(false);
        _notMetRequirementStartQuestIcon.SetActive(false);
        _notMetReuirementDoneQuestIcon.SetActive(false);

        switch (state)
        {
            case QuestState.REQUIRE_NOT_MET:
                if (startPoint) _notMetRequirementStartQuestIcon.SetActive(true);
                break;
            case QuestState.CAN_START:
                if (startPoint) _canStartQuestIcon.SetActive(true);
                break;
            case QuestState.IN_PROGRESS:
                if(endPoint) _notMetReuirementDoneQuestIcon.SetActive(true) ;
                break;
            case QuestState.CAN_FINISH:
                if (endPoint) _canDoneQuestIcon.SetActive(true);
                break;
            case QuestState.DONE:
                break;
            default:
                Debug.LogWarning("Quest state is not detect for icon: " + state);
                break;
        }
    }
}
