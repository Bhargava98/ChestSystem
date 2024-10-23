using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DotweenAnimation : MonoBehaviour
{
    public static DotweenAnimation instance;
    public GameObject chestPanel;
    public List<Sprite> availableSprites;
    public Image itemDisplayPrefab;
    public Transform itemContainer;
    public Button confirmButton;
    public Image coinImagePrefab;
    public Transform coinStartPosition;
    public Transform coinTargetPosition;
    public float itemSpacing = 120f;
    public float animationDuration = 0.5f;
    public int coinCount = 10;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        chestPanel.SetActive(false);
        confirmButton.gameObject.SetActive(false);
    }

    public void OpenChest()
    {
        chestPanel.SetActive(true);
        chestPanel.transform.localScale = Vector3.zero;

        Sequence panelSequence = DOTween.Sequence();
        panelSequence.Append(chestPanel.transform.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutBounce))
                     .OnComplete(() => StartCoroutine(ShowRewards()));
    }

    private IEnumerator ShowRewards()
    {
        List<Sprite> selectedRewards = GetRandomRewards();
        float centerX = (selectedRewards.Count - 1) * itemSpacing / -2f;

        for (int i = 0; i < selectedRewards.Count; i++)
        {
            Image newItem = Instantiate(itemDisplayPrefab, itemContainer);
            newItem.sprite = selectedRewards[i];
            newItem.color = new Color(1, 1, 1, 0);
            newItem.rectTransform.anchoredPosition = new Vector2(centerX + i * itemSpacing, -200f);

            Sequence itemSequence = DOTween.Sequence();
            itemSequence.Append(newItem.rectTransform.DOAnchorPosY(0, animationDuration).SetEase(Ease.OutBack))
                        .Join(newItem.DOFade(1f, animationDuration))
                        .Join(newItem.transform.DOScaleX(1f, animationDuration).SetEase(Ease.OutElastic));

            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(0.5f);
        ShowConfirmButton();
    }

    private List<Sprite> GetRandomRewards()
    {
        return availableSprites.OrderBy(x => Random.value).Take(3).ToList();
    }

    private void ShowConfirmButton()
    {
        confirmButton.gameObject.SetActive(true);
        confirmButton.transform.localScale = Vector3.zero;
        confirmButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
}
