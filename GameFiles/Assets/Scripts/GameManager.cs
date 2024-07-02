using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int Money;
    public Stack<GameState> state;
    public GameState currentState;
    public State[] stateScripts;
    public PlayState playState;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        stateScripts = new State[2];
        state = new Stack<GameState>();

        gameObject.AddComponent<PlayState>();
        stateScripts[0] = gameObject.GetComponent<PlayState>();
        playState = stateScripts[0] as PlayState;
        
        addState(GameState.Play);
    }

    public void addState(GameState s)
    {
        state.Push(s);
        currentState = s;
        stateScripts[(int) s].InitializeState();
    }

    public void removeState()
    {
        state.Pop();
        currentState = state.Peek();
    }

    public void changeState(GameState s)
    {
        removeState();
        addState(s);
    }

    private void Update()
    {
        stateScripts[(int) currentState].UpdateState();
    }
}
public enum GameState
{
    Play = 0,
    Paused = 1
}
