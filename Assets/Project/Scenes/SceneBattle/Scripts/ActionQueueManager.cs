using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionQueueManager : MonoBehaviour
{
    // Queue to store actions that need to be executed
    private Queue<Action> actionQueue = new Queue<Action>();
    // Variable to check if an action is currently being executed
    private bool isExecuting = false;

    void Update()
    {
        // If no action is being executed and the queue is not empty
        if (!isExecuting && actionQueue.Count > 0)
        {
            // Start executing the next action in the queue
            ExecuteNextAction();
        }
    }

    // Add an action to the queue
    public void EnqueueAction(Action action)
    {
        if (action == null)
        {
            Debug.LogError("Attempted to enqueue a null action!");
            return;
        }
        actionQueue.Enqueue(action);

        // If not currently executing an action, start executing
        if (!isExecuting)
        {
            ExecuteNextAction();
        }
    }

    // Execute the next action in the queue
    private void ExecuteNextAction()
    {
        if (actionQueue.Count == 0)
        {
            Debug.LogError("Action queue is empty!");
            isExecuting = false;
            return;
        }

        // Set the isExecuting flag to true
        isExecuting = true;

        // Get the next action from the queue
        Action action = actionQueue.Dequeue();

        if (action == null)
        {
            Debug.LogError("Dequeued action is null!");
            isExecuting = false;
            return;
        }

        // Execute the action
        action?.Invoke();

        // Reset the isExecuting flag to false after the action is executed
        //isExecuting = false;
    }

    public void SetExecutingFalse()
    {
        isExecuting = false;
    }

    public void ClearAll()
    {
        actionQueue.Clear();
    }

    #region FOR TEST
    private void CreateTestQueue()
    {
        // Example actions for testing
        EnqueueAction(() =>
        {
            isExecuting = false;
            Debug.Log("xx Start Queue");
        });

        // Add a delay action for demonstration
        EnqueueAction(() =>
        {
            StartCoroutine(DelayAction(2f, () =>
            {
                Debug.Log("xx Delayed Action Executed");
                isExecuting = false;
            }));
        });

        // Add another action to be executed after the delay
        EnqueueAction(() =>
        {
            Debug.Log("xx Last queue item");
            isExecuting = false;
        });
    }

    // Example delay coroutine
    private IEnumerator DelayAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    #endregion
}
