using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

	#region Singleton

	public static EquipmentManager instance;

	private void Awake()
	{
		instance = this;
	}

	#endregion

	public Equipment[] defaultEQ;
	public SkinnedMeshRenderer targetMesh;
	Equipment[] currentEQ;
	SkinnedMeshRenderer[] currentMeshes;

	public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
	public OnEquipmentChanged onEquipmentChanged;

	Inventory inventory;

	// Use this for initialization
	void Start () {

		inventory = Inventory.instance;

		int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
		currentEQ = new Equipment[numSlots];
		currentMeshes = new SkinnedMeshRenderer[numSlots];

		EquipDefaultItems();
	}

	public void Equip(Equipment newItem)
	{
		int slotIndex = (int)newItem.EquipSlot;

		Equipment oldItem = Unequip(slotIndex);

		if (onEquipmentChanged != null)
		{
			onEquipmentChanged.Invoke(newItem, oldItem);
		}

		SetEquipmentBlendShapes(newItem, 100);
		currentEQ[slotIndex] = newItem;

		SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.mesh);
		newMesh.transform.parent = targetMesh.transform;

		newMesh.bones = targetMesh.bones;
		newMesh.rootBone = targetMesh.rootBone;
		currentMeshes[slotIndex] = newMesh;
	}

	public Equipment Unequip(int slotIndex)
	{
		if(currentMeshes[slotIndex] != null)
		{
			Destroy(currentMeshes[slotIndex].gameObject);
		}

		if(currentEQ[slotIndex] != null)
		{
			Equipment oldItem = currentEQ[slotIndex];
			SetEquipmentBlendShapes(oldItem, 0);
			inventory.Add(oldItem);

			currentEQ[slotIndex] = null;

			if (onEquipmentChanged != null)
			{
				onEquipmentChanged.Invoke(null, oldItem);
			}
			return oldItem;
		}

		return null;
	}

	public void UnequipAll()
	{
		for (int i = 0; i < currentEQ.Length; i++)
		{
			Unequip(i);
		}
		EquipDefaultItems();
	}

	void SetEquipmentBlendShapes(Equipment item, int weight)
	{
		foreach (EquipmentMeshRegion blendShape in item.coveredMeshRegions)
		{
			targetMesh.SetBlendShapeWeight((int)blendShape, weight);
		}
	}

	void EquipDefaultItems()
	{
		foreach (Equipment item in defaultEQ)
		{
			Equip(item);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			UnequipAll();
		}
	}
}
