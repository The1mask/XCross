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
    public GameObject[] DestroyLine;                                //Line for destroy
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
    private int _gameMode;                                          /*Current GameMode: 0 - Players Place Baces
                                                                                        1 - Main GamePlay
                                                                                        3 - End of Match        */

    // Start is called before the first frame update
    void Start()
    {
        PlayerNum = 0;
        _player = 1;
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
                mapArr[i, q].GetComponent<PlayerScript>().Player = 0;
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
            Debug.Log("CliCked");
            if (Physics.Raycast(ray, out hit))
            {
                    _clicked = hit.collider.gameObject;
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
                                if (CanSpawn(_clicked))
                                {
                                    titleCoord = hit.collider.gameObject.transform.position;
                                    MenuGate(_clicked);
                                    GetArrFormObj(_clicked);
                                    _selected = mapArr[_mapX,_mapY];
                                    //mapArr[mapX, mapY] = Instantiate(_selected, titleCoord, Quaternion.identity) as GameObject;
                                    //CheckMap(mapX, mapY);
                                    //PlayerNum++;
                                    //Win();
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
                                    Debug.Log(hit.collider.gameObject.tag);
                                    GetArrFormObj(_selected);
                                    titleCoord = _selected.transform.position;
                                    Debug.Log("x"+_mapX +"Y"+ _mapY);
                                    Destroy(mapArr[_mapX, _mapY]);
                                    mapArr[_mapX, _mapY] = Instantiate(hit.collider.gameObject, titleCoord, Quaternion.identity) as GameObject;
                                }
                            }
                            break;
                    
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _pressed = false;
            //Уничтожаем Меню//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }

    void GetArrFormObj(GameObject Cell)
    {
        _mapX = Mathf.RoundToInt(Cell.transform.position.x / CellSize);
        _mapY = Mathf.RoundToInt(Cell.transform.position.y / CellSize);
    }

    void BlurStartCell(int mapX, int mapY, bool enable)
    {
        Vector4 color;
        if (_player==1)
        {
            color = new Vector4(0, 0, 255, 1);
        }
        else
        {
            color = new Vector4(255, 0, 0, 1);
        }
        if (enable)
        {
            for (int i = MapSize - 1; i >= 0; i--)
            {
                mapArr[mapX - i, mapY].GetComponent<PlayerScript>().Shining = 1;
                mapArr[mapX - i, mapY].GetComponent<PlayerScript>().SetSecColor(color);
            }
        }
        else
        {
            for (int i = MapSize - 1; i >= 0; i--)
            {
                mapArr[mapX - i, mapY].GetComponent<PlayerScript>().Shining = 0;
                mapArr[mapX - i, mapY].GetComponent<PlayerScript>().SetSecColor(new Vector4(255,255,255,1));
            }
        }
    }

    bool PlaceBace(GameObject _selected)
    {
        GetArrFormObj(_selected);
        if (_player == 1 && _mapY == 0)
        {
            Destroy(mapArr[_mapX, _mapY]);
            mapArr[_mapX, _mapY] = Instantiate(cell[1], _selected.transform.position, Quaternion.identity) as GameObject;
            mapArr[_mapX, _mapY].GetComponent<PlayerScript>().Player = 1;
            mapArr[_mapX, _mapY].GetComponent<PlayerScript>().SetMainColor(new Vector4(0, 0, 255, 1));
            _player++;
            BlurStartCell(MapSize - 1, 0, false);
            BlurStartCell(MapSize - 1, MapSize - 1, true);
        }
        else
        {
            if (_player == 2 && _mapY == (MapSize - 1))
            {
                Destroy(mapArr[_mapX, _mapY]);
                mapArr[_mapX, _mapY] = Instantiate(cell[1], _selected.transform.position, Quaternion.identity) as GameObject;
                mapArr[_mapX, _mapY].GetComponent<PlayerScript>().Player = 2;
                mapArr[_mapX, _mapY].GetComponent<PlayerScript>().SetMainColor(new Vector4(255, 0, 0, 1));
                BlurStartCell(MapSize - 1, MapSize - 1, false);
                _player--;
                return true;
            }
        }
        return false;
    }

    bool CanSpawn(GameObject Clicked) //Check, if there is an allied figure nearby, return true
    {
        GetArrFormObj(Clicked);
        
        /*for (int i = 0; i < MapSize; i++)   //Check, mapArr[x,y] = Clicked
        {
            for (int q = 0; q < MapSize; q++)
            {
                if (mapArr[i, q] == Clicked)
                {
                    _currentX = i;
                    _currentY = q;
                }
            }
        }*/

        for (int i = _mapX - 1; i < _mapX + 2; i++)       //Check current cell +/- 1 cell around
        {
            for (int q = _mapY - 1; q < _mapY + 2; q++)
            {
                if (i < 0)
                {
                    i++;
                    Debug.Log("i");
                }
                if (q < 0)
                {
                    q++;
                    Debug.Log("q");
                }
                if (i > (MapSize - 1))
                {
                    Debug.Log("false");
                    return false;
                }
                if (q > (MapSize - 1))
                {
                    q = _mapY - 1;
                    i++;
                }
                else
                {
                    if (mapArr[i, q].GetComponent<PlayerScript>().Player == _player)
                    {
                        Debug.Log(mapArr[i, q].GetComponent<PlayerScript>().Player);
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
        int startIndexX = 0;
        int endIndexX = 0;
        int startIndexY = 0;
        int endIndexY = 0;
        DestroyLine = new GameObject[3];
        if (mapX > 1 && mapX < 4)
        {
            startIndexX = -2;
            endIndexX = 3;
        }
        else
        {
            endIndexX = CheckXF(mapX);
            startIndexX = CheckXB(mapX);

        }

        if (mapY > 1 && mapY < 4)
        {
            startIndexY = -2;
            endIndexY = 3;
        }
        else
        {
            endIndexY = CheckYF(mapY);
            startIndexY = CheckYB(mapY);
        }





        for (int i = startIndexX; i < endIndexX; i++)
        {
            if (mapArr[mapX, mapY].tag == mapArr[mapX + i, mapY].tag)
            {
                DestroyLine[num] = mapArr[mapX + i, mapY];
                num = num + 1;

                if (num > 2)
                {
                    ClearLine(DestroyLine, mapArr[mapX, mapY]);
                    break;
                }


            }
            else
            {
                num = 0;
                for (int q = 0; q < DestroyLine.Length; q++)
                {
                    DestroyLine[q] = null;
                }

            }
        }

        num = 0;

        for (int i = startIndexY; i < endIndexY; i++)
        {
            if (mapArr[mapX, mapY].tag == mapArr[mapX, mapY + i].tag)
            {
                DestroyLine[num] = mapArr[mapX, mapY + i];
                num = num + 1;
                if (num > 2)
                {
                    ClearLine(DestroyLine, mapArr[mapX, mapY]);
                    break;
                }


            }
            else
            {
                num = 0;
                for (int q = 0; q < DestroyLine.Length; q++)
                {
                    DestroyLine[q] = null;
                    break;
                }
                // Debug.Log("False");
            }
        }

        if (startIndexY >= startIndexX)
        {
            startIndexX = startIndexY;
        }
        if (endIndexY <= endIndexX)
        {
            endIndexX = endIndexY;
        }

        num = 0;

        for (int i = startIndexX; i < endIndexX; i++)
        {
            if (mapArr[mapX, mapY].tag == mapArr[mapX + i, mapY + i].tag)
            {
                DestroyLine[num] = mapArr[mapX + i, mapY + i];
                num = num + 1;
                if (num > 2)
                {
                    ClearLine(DestroyLine, mapArr[mapX, mapY]);
                    break;
                }
            }
            else
            {
                num = 0;
                for (int q = 0; q < DestroyLine.Length; q++)
                {
                    DestroyLine[q] = null;
                }
            }
        }

        if (mapX == 4)
        {
            startIndexX = -1;
        }
        else
        {
            if (mapX == 5)
            {
                startIndexX = 0;
            }
            else
            {
                startIndexX = -2;
            }
        }
        if (mapX == 0)
        {
            endIndexX = 1;
        }
        else
        {
            if (mapX == 1)
            {
                endIndexX = 2;
            }
            else
            {
                endIndexX = 3;
            }
        }
        if (startIndexY >= startIndexX)
        {
            startIndexX = startIndexY;
        }
        if (endIndexY <= endIndexX)
        {
            endIndexX = endIndexY;
        }

        num = 0;

        for (int i = startIndexX; i < endIndexX; i++)
        {
            if (mapArr[mapX, mapY].tag == mapArr[mapX - i, mapY + i].tag)
            {
                DestroyLine[num] = mapArr[mapX - i, mapY + i];
                num = num + 1;
                if (num > 2)
                {
                    ClearLine(DestroyLine, mapArr[mapX, mapY]);
                    break;
                }
            }
            else
            {
                num = 0;
                for (int q = 0; q < DestroyLine.Length; q++)
                {
                    DestroyLine[q] = null;
                }
                // Debug.Log("False");
            }
        }

        return false;

    }

    int CheckXB(int mapX)
    {
        int startIndex = -2;
        if (mapX == 1)
        {
            startIndex = -1;
        }
        else
        {
            if (mapX == 0)
            {
                startIndex = 0;
            }
        }
        return startIndex;
    }
    int CheckXF(int mapX)
    {
        int endIndex = 3;
        if (mapX == 5)
        {
            endIndex = 1;
        }
        else
        {
            if (mapX == 4)
            {
                endIndex = 2;
            }
        }
        return endIndex;
    }
    int CheckYB(int mapY)
    {
        int startIndex = -2;
        if (mapY == 1)
        {
            startIndex = -1;
        }
        else
        {
            if (mapY == 0)
            {
                startIndex = 0;
            }
        }
        return startIndex;
    }
    int CheckYF(int mapY)
    {
        int endIndex = 3;
        if (mapY == 5)
        {
            endIndex = 1;
        }
        else
        {
            if (mapY == 4)
            {
                endIndex = 2;
            }
        }
        return endIndex;
    }

    void ClearLine(GameObject[] DestroyLine, GameObject current)
    {
        int _player = 0;

        if (PlayerNum % 2 == 0)
        {
            _player = 0;
        }
        else
        {
            _player = 1;
        }

        for (int i = 0; i < 3; i++)
        {
            if (DestroyLine[i] != current)
            {
                _mapX = Mathf.RoundToInt(DestroyLine[i].transform.position.x / 5.12f);
                _mapY = Mathf.RoundToInt(DestroyLine[i].transform.position.y / 5.12f);
                Destroy(DestroyLine[i]);
                mapArr[_mapX, _mapY] = Instantiate(cell[/*1*/1], DestroyLine[i].transform.position, Quaternion.identity) as GameObject;/////////////////////////////////////
                if (DestroyLine[i].layer == 8 && _player == 1)
                {
                    CurrScore[_player] += 1;
                    Debug.Log(CurrScore[_player] + _player);
                }
                if (DestroyLine[i].layer == 9 && _player == 0)
                {
                    CurrScore[_player] += 1;
                    Debug.Log(CurrScore[_player] + _player);
                }
            }
        }
        Score[_player].text = (CurrScore[_player]).ToString();
    }

    bool Win()
    {
        if (mapArr[0, 1].layer != 10 && mapArr[0, 1].layer != 8 || mapArr[1, 1].layer != 10 && mapArr[1, 1].layer != 8 || mapArr[1, 0].layer != 10 && mapArr[1, 0].layer != 8)
        {
            Title[1].text = "WIN";
        }
        if (mapArr[5, 4].layer != 10 && mapArr[5, 4].layer != 9 || mapArr[4, 5].layer != 10 && mapArr[4, 5].layer != 9 || mapArr[4, 4].layer != 10 && mapArr[4, 4].layer != 9)
        {
            Title[0].text = "WIN";
        }
        return false;
    }

    void MenuGate(GameObject _clicked)
    {
        
        if (!_phaseMenu)
        {
            for (int i = 0; i < 2; i++)
            {
                _menu[i] = Instantiate(cell[i + 2], titleCoord + new Vector3(5.12f * i, 0, -0.01f), Quaternion.identity) as GameObject;
                _menu[i].GetComponent<PlayerScript>().SetMainColor(new Vector4(0, 0, 255, 1));
                _phaseMenu = true;
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                Destroy(_menu[i]);
                _phaseMenu = false;
            }
        }
    }

}




