using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class MiniMapClickHandler : MonoBehaviour
{
    public RectTransform miniMapRectTransform; // 미니맵의 RectTransform을 에디터에서 할당
    public Camera miniMapCamera; // 미니맵 카메라를 에디터에서 할당
    public GameObject marker; // 스폰할 마커 프리팹을 에디터에서 할당
    public GameObject fire; // 스폰할 불 오브젝트 프리팹을 에디터에서 할당
    public int maxFireCount = 5; // 불 오브젝트의 최대 스폰 개수
    private GameObject currentSpawnedMarker; // 현재 스폰된 마커의 참조
    private List<GameObject> currentSpawnedFires = new List<GameObject>(); // 현재 스폰된 불 오브젝트 리스트

    int spawnObjectIndex = 0; // 스폰할 오브젝트의 타입을 결정하는 인덱스 (0: 마커, 1: 불)
    int markerstate = 0;
    private void Update()
    {
        // 왼쪽 클릭 처리: 오브젝트를 스폰
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;

            if (IsClickOnMiniMap(mousePosition))
            {
                HandleMiniMapClick(mousePosition);
            }
        }

        // 오른쪽 클릭 처리: 오브젝트를 제거
        if (Input.GetMouseButtonDown(1))
        {
            // 서버에서만 오브젝트를 제거
            if (NetworkManager.Singleton.IsServer)
            {
                if (spawnObjectIndex == 0)
                {
                    // 마커만 제거
                    RemoveCurrentSpawnedMarker();
                }
                else if (spawnObjectIndex == 1)
                {
                    // 불 오브젝트만 제거
                    RemoveAllFires();
                }
            }
        }
    }

    private bool IsClickOnMiniMap(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRectTransform, screenPosition, null, out Vector2 localPoint);
        return miniMapRectTransform.rect.Contains(localPoint);
    }

    private void HandleMiniMapClick(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRectTransform, screenPosition, null, out Vector2 localPoint);

        Vector2 normalizedPoint = new Vector2(
            (localPoint.x - miniMapRectTransform.rect.x) / miniMapRectTransform.rect.width,
            (localPoint.y - miniMapRectTransform.rect.y) / miniMapRectTransform.rect.height
        );

        Vector3 worldPosition = MiniMapToWorldPosition(normalizedPoint);
        Debug.Log("World Position: " + worldPosition);

        if (NetworkManager.Singleton.IsServer)
        {
            if (spawnObjectIndex == 0)
            {
                // 마커는 하나만 스폰되므로 기존 마커가 있으면 제거
                RemoveCurrentSpawnedMarker();
                SpawnMarkerServerRpc(worldPosition);
            }
            else if (spawnObjectIndex == 1)
            {
                // 불 오브젝트는 최대 5개까지 스폰 가능
                SpawnFireServerRpc(worldPosition);
            }
        }
    }

    private Vector3 MiniMapToWorldPosition(Vector2 normalizedPoint)
    {
        // 미니맵 카메라에서 레이 생성
        Ray ray = miniMapCamera.ViewportPointToRay(new Vector3(normalizedPoint.x, normalizedPoint.y, miniMapCamera.nearClipPlane));

        // 레이캐스트를 위한 레이캐스트 히트 정보 생성
        RaycastHit hit;
        
        // BoxCollider가 있는 오브젝트와 충돌 체크
        if (Physics.Raycast(ray, out hit))
        {
            // BoxCollider와 교차점(hit.point)을 반환
            return hit.point;
        }

        // 교차점이 없으면 기본값 반환
        return Vector3.zero;
    }

    private void SpawnMarkerAtPosition(Vector3 position)
    {
        if (marker != null)
        {
            position.y += 0.1f; // 위치 조정

            // NetworkObject를 사용하여 오브젝트를 네트워크에서 인스턴스화
            Quaternion originalRotation = marker.transform.rotation;
            GameObject spawnedMarker = Instantiate(marker, position, originalRotation);
            NetworkObject networkObject = spawnedMarker.GetComponent<NetworkObject>();
            networkObject.Spawn(); // 네트워크에서 오브젝트를 스폰
            currentSpawnedMarker = spawnedMarker; // 현재 스폰된 마커를 저장

            // 위치 고정
            spawnedMarker.transform.position = position;
        }
        else
        {
            Debug.LogWarning("No marker assigned to spawn.");
        }
    }

    private void SpawnFireAtPosition(Vector3 position)
    {
        if (fire != null)
        {
            // 기존 불 오브젝트의 개수가 최대 개수를 초과하면 가장 오래된 불 오브젝트를 제거
            if (currentSpawnedFires.Count >= maxFireCount)
            {
                RemoveOldestFire();
            }

            // NetworkObject를 사용하여 오브젝트를 네트워크에서 인스턴스화
            Quaternion originalRotation = fire.transform.rotation;
            GameObject spawnedFire = Instantiate(fire, position, originalRotation);
            NetworkObject networkObject = spawnedFire.GetComponent<NetworkObject>();
            networkObject.Spawn(); // 네트워크에서 오브젝트를 스폰
            currentSpawnedFires.Add(spawnedFire); // 스폰된 불 오브젝트를 리스트에 추가

            // 위치 고정
            spawnedFire.transform.position = position;
        }
        else
        {
            Debug.LogWarning("No fire assigned to spawn.");
        }
    }

    public void RemoveCurrentSpawnedObject()
    {
        // 현재 스폰된 마커를 제거
        if (currentSpawnedMarker != null)
        {
            NetworkObject networkObject = currentSpawnedMarker.GetComponent<NetworkObject>();
            networkObject.Despawn(); // 네트워크에서 오브젝트를 제거
            Destroy(currentSpawnedMarker);
            currentSpawnedMarker = null;
        }

        // 현재 스폰된 모든 불 오브젝트 제거
        foreach (var fireObject in currentSpawnedFires)
        {
            NetworkObject networkObject = fireObject.GetComponent<NetworkObject>();
            networkObject.Despawn(); // 네트워크에서 오브젝트를 제거
            Destroy(fireObject);
        }
        currentSpawnedFires.Clear();
    }

    private void RemoveAllFires()
    {
        foreach (var fireObject in currentSpawnedFires)
        {
            NetworkObject networkObject = fireObject.GetComponent<NetworkObject>();
            networkObject.Despawn(); // 네트워크에서 오브젝트를 제거
            Destroy(fireObject);
        }
        currentSpawnedFires.Clear();
    }

    private void RemoveOldestFire()
    {
        if (currentSpawnedFires.Count > 0)
        {
            GameObject oldestFire = currentSpawnedFires[0];
            NetworkObject networkObject = oldestFire.GetComponent<NetworkObject>();
            networkObject.Despawn(); // 네트워크에서 오브젝트를 제거
            Destroy(oldestFire);
            currentSpawnedFires.RemoveAt(0); // 리스트에서 제거
        }
    }

    private void RemoveCurrentSpawnedMarker()
    {
        if (currentSpawnedMarker != null)
        {
            NetworkObject networkObject = currentSpawnedMarker.GetComponent<NetworkObject>();
            networkObject.Despawn(); // 네트워크에서 오브젝트를 제거
            Destroy(currentSpawnedMarker);
            currentSpawnedMarker = null;
        }
    }

    public void SetSpawnObjectIndex(int index)
    {
        // index 값을 받아서 spawnObjectIndex 값을 설정
        spawnObjectIndex = index;

        if (index == 0)
        {
            Debug.Log("Marker selected for spawning.");
        }
        else if (index == 1)
        {
            Debug.Log("Fire selected for spawning.");
        }
    }
    [ServerRpc]
    private void SpawnMarkerServerRpc(Vector3 position)
    {
        SpawnMarkerAtPosition(position);
    }

    [ServerRpc]
    private void SpawnFireServerRpc(Vector3 position)
    {
        SpawnFireAtPosition(position);
    }
}
