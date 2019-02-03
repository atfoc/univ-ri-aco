using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiControler : MonoBehaviour
{


	PathControler pathControler;
	CityControler cityControler;
	AcoControler acoControler;
	AnimationControler animationControler;
	private PlayerUiControler ui;
	public GameObject player;
	private string format_ = "{0}/{1}";

	public Slider sliderAntNumber;
	public Text labelAntNumber;

	public Slider sliderCoefficient;
	public Text labelCoefficient;
	public Slider sliderPheremont;
	public Text labelPheremont;
	public Slider sliderIteration;
	public Text labelIteration;
	public Slider sliderAplha;
	public Text labelAlpha;
	public Slider sliderBeta;
	public Text labelBeta;
	public Slider sliderAntSpeed;
	public Text labelAntSpeed;

	void Start()
	{

		pathControler = gameObject.GetComponent<PathControler>();
		cityControler = gameObject.GetComponent<CityControler>();
		acoControler = gameObject.GetComponent<AcoControler>();
		animationControler = gameObject.GetComponent<AnimationControler>();
		dataInit();
		ui = player.GetComponent<PlayerUiControler>();
	}

	// Update is called once per frame
	void Update()
	{

	}


	void dataInit()
	{

		labelAntNumber.text = string.Format(format_, acoControler.antNumber,
		 (int)sliderAntNumber.maxValue);
		sliderAntNumber.value = acoControler.antNumber;

		labelAntSpeed.text = string.Format(format_, acoControler.antSpeed,
		(int)sliderAntSpeed.maxValue);
		sliderAntSpeed.value = acoControler.antSpeed;

		labelCoefficient.text = string.Format(format_, acoControler.coefficient,
		sliderCoefficient.maxValue);
		sliderCoefficient.value = acoControler.coefficient;

		labelPheremont.text = string.Format(format_, acoControler.feremonAmount,
		sliderPheremont.maxValue);
		sliderPheremont.value = acoControler.feremonAmount;

		labelIteration.text = string.Format(format_, acoControler.iterationCount,
		(int)sliderIteration.maxValue);
		sliderIteration.value = acoControler.iterationCount;

		labelAlpha.text = string.Format(format_, acoControler.alpha, sliderAplha.maxValue);
		sliderAplha.value = acoControler.alpha;

		labelBeta.text = string.Format(format_, acoControler.beta, sliderBeta.maxValue);
		sliderBeta.value = acoControler.beta;
	}
	public void onTownValueChanged(int index)
	{
		CityControler.Group g = CityControler.Group.All;

		if (index == 1)
		{
			g = CityControler.Group.Large;
		}
		else if (index == 2)
		{

			g = CityControler.Group.Medium;
		}
		else if (index == 3)
		{
			g = CityControler.Group.Small;
		}
		else if (index == 4)
		{

			g = CityControler.Group.Random;
		}

		cityControler.setActiveGroup(g);

	}

	public void onColorChanged(int index)
	{

		Color c = new Color();
		if (index == 0)
		{
			c = new Color(1, 0, 0, 1);
		}
		else if (index == 1)
		{
			c = new Color(0, 1, 0, 1);
		}
		else if (index == 2)
		{
			c = new Color(0, 0, 1, 1);
		}

		pathControler.color = c;
	}

	public void onNumberChanged()
	{
		acoControler.antNumber = (int)sliderAntNumber.value;
		labelAntNumber.text = string.Format(format_, (int)sliderAntNumber.value,
		 (int)sliderAntNumber.maxValue);


	}

	public void onSpeedChanged()
	{
		acoControler.antSpeed = (int)sliderAntSpeed.value;
		labelAntSpeed.text = string.Format(format_, (int)sliderAntSpeed.value,
		(int)sliderAntSpeed.maxValue);

	}

	public void onCoefficientChanged()
	{

		acoControler.coefficient = sliderCoefficient.value;
		labelCoefficient.text = string.Format(format_, sliderCoefficient.value,
		sliderCoefficient.maxValue);
	}


	public void onPheremontChanged()
	{

		acoControler.feremonAmount = sliderPheremont.value;
		labelPheremont.text = string.Format(format_, sliderPheremont.value,
		sliderPheremont.maxValue);
	}

	public void onIterationChanged()
	{
		acoControler.iterationCount = (int)sliderIteration.value;
		labelIteration.text = string.Format(format_, (int)sliderIteration.value,
		(int)sliderIteration.maxValue);
	}

	public void onAlphaChanged()
	{
		acoControler.alpha = sliderAplha.value;
		labelAlpha.text = string.Format(format_, sliderAplha.value, sliderAplha.maxValue);
	}

	public void onBetaChanged()
	{
		acoControler.beta = sliderBeta.value;
		labelBeta.text = string.Format(format_, sliderBeta.value, sliderBeta.maxValue);
	}

	public void onStartClicked()
	{
		ui.toggleUi();
		acoControler.startAlgorithm();
	}

	public void onStopCliked()
	{
		acoControler.stopAlgorithm();
		animationControler.reset();
		acoControler.stoped = true;
	}

}
