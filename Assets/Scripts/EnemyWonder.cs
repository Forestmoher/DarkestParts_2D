using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyWonder : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed;
    [SerializeField] private GameObject[] _pathStops;
    [SerializeField] private int _pauseTime;
    [SerializeField] private ActorAnimator _animator;


    //follow set path 
    private async void Wonder()
    {
        foreach(var stop in _pathStops)
        {
            await MoveToStop(stop);
            await PauseAtStop();
        }
        await Task.Yield();
        Wonder();
    }

    private async Task MoveToStop(GameObject stop)
    {
        _animator.SetAnimation(stop.transform.position - transform.position);
        LeanTween.move(gameObject, stop.transform, _speed);
        await Task.Delay((int)(_speed * 1000));
    }

    private async Task PauseAtStop()
    {
        _animator.SetAnimation(Vector2.zero);
        await Task.Delay(_pauseTime * 1000);
    }

    private void Awake()
    {
        Wonder();
    }

    //private void OnDisable()
    //{
    //    LeanTween.cancel(gameObject);
    //}



    //cone of vision: if player enters, trigger combat


}
