using System.Collections.Generic;
using Colyseus;
using UnityEngine;


public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{
    [field: SerializeField] public LossCounter _lossCounter { get; private set; }
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private EnemyController _enemy;

    private ColyseusRoom<State> _room;
    private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();
    protected override void Awake()
    {
        base.Awake();
        Instance.InitializeClient(); //W1L5.2
        Connect();
    }

    private async void Connect() //асинхронный метод, чтобы можно было дождаться пока что-то выполнится
    {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "speed",  _player.speed },
            { "hp",  _player.maxHealth }
        };

        _room = await Instance.client.JoinOrCreate<State>("state_handler", data); //await - дожидаемся пока произойдет присоединение к комнате сервера
        _room.OnStateChange += OnChange;

        _room.OnMessage<string>("Shoot", ApplyShoot);
    }

    private void ApplyShoot(string jsonShootInfo)
    {
        ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);
        if (_enemies.ContainsKey(shootInfo.key) == false)
        {
            Debug.LogError("Enemy нет, а он пытался стрелять");
            return;
        }
        _enemies[shootInfo.key].Shoot(shootInfo);


    }

    private void OnChange(State state, bool isFirstState)
    {
        if (isFirstState == false) return;



        //state.players.ForEach(ForEachEnemy);
        state.players.ForEach((key, player) =>
        {
            if (key == _room.SessionId) CreatePlayer(player);
            else CreateEnemy(key, player);
        });

        _room.State.players.OnAdd += CreateEnemy;
        _room.State.players.OnRemove += RemoveEnemy;
    }
    private void CreatePlayer(Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ); //W1L6
        var playerCharacter = Instantiate(_player, position, Quaternion.identity);
        player.OnChange += playerCharacter.OnChange;
        _room.OnMessage<string>("Restart", playerCharacter.GetComponent<Controller>().Restart);
    }
    
    private void CreateEnemy(string key, Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ); //W1L6
        var enemy = Instantiate(_enemy, position, Quaternion.identity);
        enemy.Init(key, player);

        _enemies.Add(key, enemy);
    }
    private void RemoveEnemy(string key, Player player)
    {
        if (_enemies.ContainsKey(key) == false) return;

        var enemy = _enemies[key];
        enemy.Destroy();
        _enemies.Remove(key);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _room.Leave();
    }
    public void SendMessage(string key, Dictionary<string, object> data)
    {
        _room.Send(key, data);
    }
    public void SendMessage(string key, string data)
    {
        _room.Send(key, data);
    }
    public string GetSessionID() => _room.SessionId;
    
}

//public class Room
//{
//    public State1 c1;
//}

//public class State1
//{
//    public float score;
//    public float time;
//}
