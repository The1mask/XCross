using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    public GameObject[,] mapArr;                                    //Game map array
    public GameObject[] cell;                                       //Game cells. Cell[0] = empty cell; Cell[1] = player bace; Cell[3] = o; Cell[4] = x;
    private GameObject _clicked;                                    //Clicked at Scene
    private GameObject _selected;                                   //Selected at mapArr
    public List<GameObject> DestroyLine;                                //Line for destroy
    public Text[] Title;                                            //Text for Win/Lose Message
    public Text[] Score;                                            //Players Score Text
    private int[] CurrScore;                                        //Current Score
    static private float CellSize = 5.12f;                          //Size of Cell
    private bool _pressed;                                          //Check Pressed?
    public int PlayerAmount;                                        //Players Amount
    private int _mapX;                                                //X for mapArr
    private int _mapY;                                                //Y for mapArr
    public int PlayerNum;                                           //Stroke number
    public int _player;                                             //Player number
    private GameObject[] _menu;                                     //Menu Array
    private bool _phaseMenu;                                        //Phase menu close/open
    public int MapSize;                                             //Map size :|
    public Vector3 titleCoord;                                      //Coordinats of cell at Scene
    private Color[] _playerColor;
    private int _gameMode;                                          /*Current GameMode: 0 - Players Place Baces
                                                                                        1 - Main GamePlay
                                                                                        3 - End of Match        */

    // Start is called before the first frame update
    void Start()
    {
        _playerColor = new Color[2] { new Vector4(0, 0, 255, 1), new Vector4(255, 0, 0, 1) }; 
        _player = 0;
        MapSize = 6;
        mapArr = new GameObject[MapSize, MapSize];
        CurrScore = new int[PlayerAmount];
        _menu = new GameObject[PlayerAmount];

        for (int i = 0; i < PlayerAmount; i++)
        {
            CurrScore[i] = 0;
            Score[i].text = CurrScore[i].ToString();
        }

        for (int i = 0; i < MapSize; i++)
        {
            for (int q = 0; q < MapSize; q++)
            {
                mapArr[i, q] = Instantiate(cell[0], new Vector3(i * 5.12F, q * 5.12f, 0), Quaternion.identity) as GameObject;
                mapArr[i, q].transform.GetChild(1).GetComponent<PlayerScript>().Player = 0;
            }
        }
        BlurStartCell(MapSize - 1, 0, true);
    }



    void Update()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                    _clicked = hit.collider.transform.parent.gameObject;
                    switch (_gameMode)
                    {
                        case 0:
                            if (PlaceBace(_clicked))
                            {
                                _gameMode += 1;
                            }
                            
                            break;
                        case 1:
                            
                            if (!_phaseMenu)
                            {
                                if (hit.collider.gameObject.tag == "0")
                                {
                                    if (CanSpawn(_clicked))
                                    {
                                        titleCoord = hit.collider.gameObject.transform.position;
                                        MenuGate(_clicked);
                                        GetArrFormObj(_clicked);
                                        _selected = hit.collider.transform.parent.gameObject;
                                    }
                                }
                            }
                            else  
                            {
                                if (hit.collider.gameObject.tag == "0")
                                {
                                    MenuGate(_clicked);
                                }
                                else
                                {
                                    SpawnPlayerCell(hit.collider.transform.parent.gameObject);
                                }
                            }
                            break;
                    
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _pressed = false;
           
        }
    }

    void SpawnPlayerCell(GameObject Clicked)
    {
        GetArrFormObj(_selected);
        titleCoord = _selected.transform.position;
        Destroy(mapArr[_mapX, _mapY]);
        mapArr[_mapX, _mapY] = Clicked;
        Clicked.transform.position = titleCoord;
        _selected = mapArr[_mapX, _mapY];
        if (_player == 1)
        {
            mapArr[_mapX, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().Player = _player;
            _player--;
        }
        else
        {
            mapArr[_mapX, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().Player = _player;
            _player++;
        }
        CheckMap(_mapX, _mapY);
        MenuGate(_clicked);
        Win();
    }

    void GetArrFormObj(GameObject Cell)
    {
        _mapX = Mathf.RoundToInt(Cell.transform.position.x / CellSize);
        _mapY = Mathf.RoundToInt(Cell.transform.position.y / CellSize);
    }

    void BlurStartCell(int mapX, int mapY, bool enable)
    {
        Vector4 color;
        if (_player==0)
        {
            color = new Vector4(0, 0, 155, 1);
        }
        else
        {
            color = new Vector4(155, 0, 0, 1);
        }
        if (enable)
        {
            for (int i = MapSize - 1; i >= 0; i--)
            {
                mapArr[mapX - i, mapY].transform.GetChild(1).GetComponent<PlayerScript>().SetSecColor(color, true);
               
            }
        }
        else
        {
            for (int i = MapSize - 1; i >= 0; i--)
            {
                mapArr[mapX - i, mapY].transform.GetChild(1).GetComponent<PlayerScript>().SetSecColor(new Vector4(255,255,255,1), false);
            }
        }
    }

    bool PlaceBace(GameObject _selected)
    {
        GetArrFormObj(_selected);
        if (_player == 0 && _mapY == 0)
        {
            BlurStartCell(MapSize - 1, 0, false);
            Destroy(mapArr[_mapX, _mapY]);
            mapArr[_mapX, _mapY] = Instantiate(cell[1], _selected.transform.position, Quaternion.identity) as GameObject;
            mapArr[_mapX, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().Player = _player;
            mapArr[_mapX, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().Bace = true;
            mapArr[_mapX, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().SetMainColor(_playerColor[_player]);
             _player++;
            BlurStartCell(MapSize - 1, MapSize - 1, true);
            
            
        }
        else
        {
            if (_player == 1 && _mapY == (MapSize - 1))
            {
                BlurStartCell(MapSize - 1, MapSize - 1, false);
                Destroy(mapArr[_mapX, _mapY]);
                mapArr[_mapX, _mapY] = Instantiate(cell[1], _selected.transform.position, Quaternion.identity) as GameObject;
                mapArr[_mapX, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().Player = _player;
                mapArr[_mapX, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().Bace = true;
                mapArr[_mapX, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().SetMainColor(_playerColor[_player]);
                
                _player--;
                return true;
            }
        }
        return false;
    }

    bool CanSpawn(GameObject Clicked) //Check, if there is an allied figure nearby, return true
    {
        GetArrFormObj(Clicked);

        for (int i = _mapX - 1; i <= _mapX + 1; i++)       //Check current cell +/- 1 cell around
        {
            for (int q = _mapY - 1; q <= _mapY + 1; q++)
            {
                if (i < 0)
                {
                    i++;
                }
                if (q < 0)
                {
                    q++;
                    
                }
                if (i > (MapSize - 1))
                {
                    return false;
                }
                if (q > (MapSize - 1))
                {
                    break;
                }
                else
                {
                    Debug.Log(_player);
                    if (mapArr[i, q].transform.GetChild(1).GetComponent<PlayerScript>().Player == _player)
                    {
                        return true;
                    }
                    if (mapArr[i, q].transform.GetChild(1).GetComponent<PlayerScript>().Player != _player && Clicked.transform.GetChild(1).GetComponent<PlayerScript>().Bace)
                    {
                        return true;
                    }
                }
                
                
            }
        }
        return false;
    }


    bool CheckMap(int mapX, int mapY)
    {
        int num = 0;
        int _savednum = 0;
        int startIndexX = 0 - _mapX;
        int endIndexX = 5 - _mapX;
        int startIndexY = 0 - _mapY;
        int endIndexY = 5 - _mapY;
        DestroyLine = new List<GameObject>();

        num = CheckX(startIndexX, endIndexX, num);
        num = CheckY(startIndexY, endIndexY, num);
        num = CheckXY(startIndexX, endIndexX, startIndexY, endIndexY, num);
        num = CheckYX(startIndexX, endIndexX, startIndexY, endIndexY, num);

        if (num > 2)
        {
            ClearLine(num);
        }
        return false;

    }

    int CheckX(int startIndexX, int endIndexX, int num)
    {
        int localnum = 0;
        for (int i = startIndexX; i <= endIndexX; i++)
        {
            if (mapArr[_mapX, _mapY].transform.GetChild(1).tag == mapArr[_mapX + i, _mapY].transform.GetChild(1).tag && mapArr[_mapX + i, _mapY].transform.GetChild(1).GetComponent<PlayerScript>().Bace == false)
            {
                if (!DestroyLine.Contains(mapArr[_mapX + i, _mapY]))
                {
                    DestroyLine.Add(mapArr[_mapX + i, _mapY]);
                    localnum++;
                }
                if (DestroyLine.Count > 2 && localnum > 2)
                    num = DestroyLine.Count;
            }
            else
            {
                if (DestroyLine.Count % 3 != 0 && DestroyLine.Count != 0)
                {
                    for (int q = DestroyLine.Count-1; q > num - 1; q--)
                    {
                        DestroyLine.RemoveAt(q);
                    }
                    localnum = 0;
                }
            }
        }
            return num;

    }

    int CheckY(int startIndexY, int endIndexY, int num)
    {
        int localnum = 0;
        for (int i = startIndexY; i <= endIndexY; i++)
        {
            if (mapArr[_mapX, _mapY].transform.GetChild(1).tag == mapArr[_mapX, _mapY + i].transform.GetChild(1).tag && mapArr[_mapX, _mapY + i].transform.GetChild(1).GetComponent<PlayerScript>().Bace == false)
            {
                if (!DestroyLine.Contains(mapArr[_mapX, _mapY + i]))
                {
                    DestroyLine.Add(mapArr[_mapX, _mapY + i]);
                    localnum++;
                }
                if (DestroyLine.Count > 2 && localnum > 2)
                    num = DestroyLine.Count;
            }
            else
            {
                if (DestroyLine.Count % 3 != 0 && DestroyLine.Count != 0)
                {
                    for (int q = DestroyLine.Count - 1; q > num - 1; q--)
                    {
                        DestroyLine.RemoveAt(q);
                    }
                    localnum = 0;
                }
            }
        }
            return num;
    }

    int CheckXY(int startIndexX, int endIndexX,int startIndexY,int endIndexY, int num)
    {
        int localnum = 0;
        if (startIndexX < startIndexY)
        {
            startIndexX = startIndexY;
        }
        if (endIndexX > endIndexY)
        {
            endIndexX = endIndexY;
        }
        for (int i = startIndexX; i <= endIndexX; i++)
        {
            if (mapArr[_mapX, _mapY].transform.GetChild(1).tag == mapArr[_mapX + i, _mapY + i].transform.GetChild(1).tag && mapArr[_mapX + i, _mapY + i].transform.GetChild(1).GetComponent<PlayerScript>().Bace == false)
            {
                if (!DestroyLine.Contains(mapArr[_mapX + i, _mapY + i]))
                {
                    DestroyLine.Add(mapArr[_mapX + i, _mapY + i]);
                    localnum++;
                }
                if (DestroyLine.Count > 2 && localnum > 2)
                    num = DestroyLine.Count;
            }
            else
            {
                if (DestroyLine.Count % 3 != 0 && DestroyLine.Count != 0)
                {
                    for (int q = DestroyLine.Count - 1; q > num - 1 ; q--)
                    {
                        DestroyLine.RemoveAt(q);
                    }
                    localnum = 0;
                }
            }
        }
            return num;
    }

    int CheckYX(int startIndexX, int endIndexX, int startIndexY, int endIndexY, int num)
    {
        int localnum = 0;
        if (Mathf.Abs(startIndexX) >= endIndexY)
        {
            startIndexX = endIndexY;
        }
        if (Mathf.Abs(startIndexY) <= endIndexX)
        {
            endIndexX = startIndexY;
        }
        for (int i = endIndexX; i <= Mathf.Abs(startIndexX); i++)
            {
                if (mapArr[_mapX, _mapY].transform.GetChild(1).tag == mapArr[_mapX - i, _mapY + i].transform.GetChild(1).tag && mapArr[_mapX - i, _mapY + i].transform.GetChild(1).GetComponent<PlayerScript>().Bace == false)
                {
                    if (!DestroyLine.Contains(mapArr[_mapX - i, _mapY + i]))
                    {
                        DestroyLine.Add(mapArr[_mapX - i, _mapY + i]);
                        localnum++;
                    }
                    if (DestroyLine.Count > 2 && localnum > 2)
                        num = DestroyLine.Count;
                }
                else
                {
                    if (DestroyLine.Count % 3 != 0 && DestroyLine.Count != 0)
                    {
                        for (int q = DestroyLine.Count - 1; q > num - 1; q--)
                        {
                            DestroyLine.RemoveAt(q);
                        }
                        localnum = 0;
                    }
                }
            }
        return num;
    }

    void ClearLine(int num)
    {
        if (num==3)
        {
            foreach (GameObject line in DestroyLine)
            {
                if (line != _selected)
                {
                    Destroy(line);
                    GetArrFormObj(line);
                    mapArr[_mapX, _mapY] = Instantiate(cell[0], line.transform.position, Quaternion.identity) as GameObject;
                }
            }
        }
        else
        {
            foreach (GameObject line in DestroyLine)
            {
                if (line != _selected && line.transform.GetChild(1).GetComponent<PlayerScript>().Player != _selected.transform.GetChild(1).GetComponent<PlayerScript>().Player)
                {
                    Destroy(line);
                    GetArrFormObj(line);
                    mapArr[_mapX, _mapY] = Instantiate(cell[0], line.transform.position, Quaternion.identity) as GameObject;
                }
            }
        }
    }
 

    bool Win()
    {
        if (CanSpawn(_selected))
        {
            Title[_player].text = "WIN";
        }
        return false;
    }

    void MenuGate(GameObject _clicked)
    {
        
        if (!_phaseMenu)
        {
            if (_clicked.transform.position.x<25)
            {
                for (int i = 0; i < 2; i++)
                {
                    _menu[i] = Instantiate(cell[i + 2], titleCoord + new Vector3(5.12f * i, 0, -0.02f), Quaternion.identity) as GameObject;
                    _menu[i].transform.GetChild(1).GetComponent<PlayerScript>().SetMainColor(_playerColor[_player]);
                    _phaseMenu = true;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    _menu[i] = Instantiate(cell[i + 2], titleCoord + new Vector3(5.12f * -i, 0, -0.02f), Quaternion.identity) as GameObject;
                    _menu[i].transform.GetChild(1).GetComponent<PlayerScript>().SetMainColor(_playerColor[_player]);
                    _phaseMenu = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                if (_clicked!= _menu[i])
                {
                    Destroy(_menu[i]);
                    _phaseMenu = false;
                }
            }
        }
    }

}




