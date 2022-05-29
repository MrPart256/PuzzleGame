using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleGameController : MonoBehaviour
{

    public static PuzzleGameController Instance;

    [SerializeField] private GameObject puzzlePrefab;
    [SerializeField] private RectTransform puzzleSpawnRect;
    [SerializeField] private RectTransform puzzleTargetRect;
    [SerializeField] private GameObject winScreen;

    private const string url = "http://placekitten.com/650/650";

    private HashSet<GameObject> completedPuzzleElements; //Сделал хэшсет для исключения добавления повторяющихся пазлов
    private List<Sprite> puzzleElements;
    private List<Vector2> initialPositions;
    private List<Vector2> RandomizedPositions;

    private void Awake()
    {
        Instance = this;
        puzzleElements = new List<Sprite>();
        initialPositions = new List<Vector2>();
        completedPuzzleElements = new HashSet<GameObject>();

        SetPuzzles();
    }

    private async void SetPuzzles()
    {
        Texture2D downloadedTex = await DownloadImage(url);

        int puzzleElementWidth = downloadedTex.width / 3;
        int puzzleElementHeight = downloadedTex.height / 3;

       

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                puzzleElements.Add(CropSprite(downloadedTex, puzzleElementWidth * x, puzzleElementHeight * y, puzzleElementWidth, puzzleElementHeight)); //Создаем список порезанных спрайтов с картинки
                initialPositions.Add(new Vector2((puzzleElementWidth * x)- puzzleElementWidth, (puzzleElementHeight * y)- puzzleElementHeight)); //Задаем изначальные позиции этих спрайтов
            }
        }

        RandomizedPositions = initialPositions.OrderBy(item => Random.Range(0, initialPositions.Count)).ToList(); //Рандомим порядок позиций будущих пазлов

        for (int i = 0; i < puzzleElements.Count; i++) //Спавним пазлы
        {
            var puzzle = Instantiate(puzzlePrefab, puzzleSpawnRect.transform);
            var controllerComponent = puzzle.GetComponent<PuzzleElementController>();
            var imageComponent = puzzle.GetComponent<Image>();

            imageComponent.sprite = puzzleElements[i];
            imageComponent.SetNativeSize();

            controllerComponent.InitialPosition = initialPositions[i];
            controllerComponent.SetCurrentPosition(RandomizedPositions[i]);
        }
    }

    private Sprite CropSprite(Texture2D tex, int x1, int y1, int x2, int y2)
    {
        Sprite croppedSprite = Sprite.Create(tex, new Rect(x1, y1, x2, y2), Vector2.one * 0.5f, 100);
        return croppedSprite;
    }

    private async Task<Texture2D> DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture($"{url}?image={Random.Range(0, 16)}");

        var sendRequest = request.SendWebRequest();
        while (sendRequest.isDone == false)
            await Task.Delay(1000 / 30);

        if (request.result != UnityWebRequest.Result.Success)
        {
            return null;
        }

        return DownloadHandlerTexture.GetContent(request);
    }

    public  void AddCompletedPuzzleElementToList(GameObject puzzleElement)
    {
        completedPuzzleElements.Add(puzzleElement);

        if(completedPuzzleElements.Count == 9)
        {
            winScreen.SetActive(true);
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
