using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
  [SerializeField] private float timeLimit = 120f; // In seconds
  [SerializeField] private UnityEvent expire;

  private ProgressBar progressBar;

  private float currentTime;
  private bool isExpired;

  private void Start()
  {
    progressBar = GetComponentInChildren<ProgressBar>();
    progressBar.SetMaxValue(timeLimit);
    progressBar.SetValue(timeLimit);

    currentTime = timeLimit;
  }

  private void Update()
  {
    if (!isExpired)
    {
      currentTime -= Time.deltaTime;
      if (currentTime <= 0)
      {
        currentTime = 0;
        expire.Invoke();
        isExpired = true;
      }
    }

    progressBar.SetValue(currentTime);
  }
}
