using System.Collections.Generic;
using UnityEngine;

public class CandyPool : MonoBehaviour
{
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private int        initialPoolSize = 80;
    [SerializeField] private Transform  poolParent;

    private readonly Queue<CandyBehaviour> _pool = new Queue<CandyBehaviour>();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
            _pool.Enqueue(CreateNew());
    }

    public CandyBehaviour Get(CandyData data, Vector3 worldPos)
    {
        CandyBehaviour candy = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        candy.transform.SetParent(null);
        candy.transform.position = worldPos;
        candy.Initialize(data);
        return candy;
    }

    public void Return(CandyBehaviour candy)
    {
        candy.ResetForPool();
        candy.transform.SetParent(poolParent);
        _pool.Enqueue(candy);
    }

    private CandyBehaviour CreateNew()
    {
        GameObject go = Instantiate(candyPrefab, poolParent);
        go.SetActive(false);
        return go.GetComponent<CandyBehaviour>();
    }
}
