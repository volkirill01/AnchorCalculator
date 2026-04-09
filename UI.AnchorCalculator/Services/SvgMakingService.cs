using Core.AnchorCalculator.Entities;
using GrapeCity.Documents.Svg;
using System.Drawing;
using System.Text;

namespace UI.AnchorCalculator.Services;

public class SvgMakingService
{
	const int X_INITIAL_COORD = 325;
	const int Y_INITIAL_COORD = 100;
	const int VIEW_WIDTH_PIXELS = 900;
	const int VIEW_HEIGHT_PIXELS = 1200;
	const int WIDTH_PIXELS = 900;
	const int HEIGHT_PIXELS = 900;
	const int ANCHOR_LENGTH_MAX = 700;
	const int ANCHOR_SECOND_LENGTH_MAX = 500;
	const int ANCHOR_BEND_LENGTH_MAX = 300;

	private float m_XOrigin = X_INITIAL_COORD;
	private float m_YOrigin = Y_INITIAL_COORD;
	private float m_ScaledThreadLength;
	private float m_ScaledSecondThreadLength;

	public void GetSvgStraightAnchor(Anchor anchor)
	{
		string iconDiameter = anchor.Material.TypeId == 1 ? "Арм" : "⌀";

		if (anchor.BendLengthMillimeters <= ANCHOR_BEND_LENGTH_MAX)
			m_XOrigin += anchor.BendLengthMillimeters;
		else
			m_XOrigin += ANCHOR_BEND_LENGTH_MAX;

		GetScaledLength(anchor.ThreadLengthMillimeters, anchor.ThreadSecondLengthMillimeters);

		int gap = 20; // Gap in out of max LengthMillimeters  of anchor
		int outPartHorSize = 45; // LengthMillimeters output part of horizontal size
		int outPartRadSize = 45; // LengthMillimeters of shelf of radius size

		var svgDoc = new GcSvgDocument();
		svgDoc.RootSvg.Width = new SvgLength(WIDTH_PIXELS, SvgLengthUnits.Pixels);
		svgDoc.RootSvg.Height = new SvgLength(HEIGHT_PIXELS, SvgLengthUnits.Pixels);

		List<SvgElement> svgElements = new(); // Make list to fill with objects SvgRectElement

		// Draw part with thread
		var rectThreadBodyAnchor = GetSvgRectElement(m_XOrigin,
			m_YOrigin,
			anchor.ThreadDiameterMillimeters,
			m_ScaledThreadLength,
			Color.Transparent,
			Color.Black,
			1.5f,
			SvgLengthUnits.Pixels);

		svgElements.Add(rectThreadBodyAnchor);

		var rectThreadStepBodyAnchor = GetSvgRectElement(m_XOrigin + anchor.ThreadStepMillimeters / 2,
			m_YOrigin,
			anchor.ThreadDiameterMillimeters - anchor.ThreadStepMillimeters,
			m_ScaledThreadLength,
			Color.Transparent,
			Color.Black,
			1f,
			SvgLengthUnits.Pixels);

		svgElements.Add(rectThreadStepBodyAnchor);

		if (anchor.ThreadLengthMillimeters > 0)
		{
			// Draw sizes of part with thread

			// Size of thread's diametr

			var lineVertLeftSizeDiamThread = GetSvgLineElement(m_XOrigin,
			m_YOrigin,
			m_XOrigin,
			m_YOrigin - outPartHorSize,
			Color.Black,
			0.5f,
			SvgLengthUnits.Pixels);

			svgElements.Add(lineVertLeftSizeDiamThread);

			var lineVertRightSizeDiamThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
					m_YOrigin,
					m_XOrigin + anchor.ThreadDiameterMillimeters,
					m_YOrigin - outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineVertRightSizeDiamThread);

			var lineHorSizeDiamThread = GetSvgLineElement(m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						m_XOrigin + anchor.ThreadDiameterMillimeters + 105,
						m_YOrigin - (outPartHorSize - 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorSizeDiamThread);

			var lineSerifLeftSizeDiamThread = GetSerif(m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifLeftSizeDiamThread);

			var lineSerifRightSizeDiamThread = GetSerif(m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin - (outPartHorSize - 5),
							m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin - (outPartHorSize - 5),
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifRightSizeDiamThread);

			svgElements.Add(GetSvgTextElement($"М{anchor.ThreadDiameterMillimeters}x{anchor.ThreadStepMillimeters}",
							m_XOrigin + anchor.ThreadDiameterMillimeters + 8,
							m_YOrigin - (outPartHorSize - 3),
							0,
							SvgLengthUnits.Pixels)); // Make text of size's value diametr of thread

			// Size of thread's LengthMillimeters

			var lineHorTopSizeLengthThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
						m_YOrigin,
						m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthThread);

			var lineHorBotSizeLengthThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength,
							m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
							m_YOrigin + m_ScaledThreadLength,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineHorBotSizeLengthThread);

			var lineVerSizeDiamThread = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin,
									m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

			svgElements.Add(lineVerSizeDiamThread);

			var lineSerifTopSizeDiamThread = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
								m_YOrigin,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
								m_YOrigin,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeDiamThread);

			var lineSerifBotSizeDiamThread = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeDiamThread);

			svgElements.Add(GetSvgTextElement($"{anchor.ThreadLengthMillimeters}",
				m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize - 2,
				m_YOrigin + m_ScaledThreadLength / 2 + 10,
				-90,
				SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of thread
		}

		// Size of anchors's diametr

		var lineHorSizeDiamAnchor = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2 + 55,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineHorSizeDiamAnchor);

		var lineSerifLeftSizeDiamAnchor = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineSerifLeftSizeDiamAnchor);

		var lineSerifRightSizeDiamAnchor = GetSerif(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineSerifRightSizeDiamAnchor);

		svgElements.Add(GetSvgTextElement($"{iconDiameter}{anchor.DiameterMillimeters}",
			m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2 + 5,
			m_YOrigin + m_ScaledThreadLength + outPartHorSize - 2,
			0,
			SvgLengthUnits.Pixels)); // Make text of size's value diametr of anchor

		// Make object basic part without thread and bend

		SvgRectElement rectBasicBodyAnchor;

		// Make objects of sizes anchor's LengthMillimeters

		SvgLineElement lineHorTopSizeLengthOfAnchor;
		SvgLineElement lineHorBotSizeLengthOfAnchor;
		SvgLineElement lineVertSizeLengthOfAnchor;
		SvgLineElement lineAxialTopHalfOfAnchor;
		SvgLineElement lineAxialBotHalfOfAnchor;

		// Make objects of part with second anchor's thread

		SvgRectElement rectThreadSecondBodyAnchor;
		SvgRectElement rectThreadSecondStepBodyAnchor;

		// Make objects of secont thread's diametr

		SvgLineElement lineHorTopSizeLengthThreadSecond;
		SvgLineElement lineHorBotSizeLengthThreadSecond;
		SvgLineElement lineVerSizeDiamThreadSecond;
		SvgLineElement lineSerifTopSizeDiamThreadSecond;
		SvgLineElement lineSerifBotSizeDiamThreadSecond;

		// Make object bending part without radius

		// SvgRectElement rectBendAnchor;

		if (anchor.LengthMillimeters <= ANCHOR_LENGTH_MAX)
		{
			if (anchor.ThreadSecondLengthMillimeters > 0)
			{
				// Size of second thread's LengthMillimeters

				lineHorTopSizeLengthThreadSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin + anchor.LengthMillimeters - m_ScaledSecondThreadLength,
							m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
							m_YOrigin + anchor.LengthMillimeters - m_ScaledSecondThreadLength,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthThreadSecond);

				lineHorBotSizeLengthThreadSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
								m_YOrigin + anchor.LengthMillimeters,
								m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
								m_YOrigin + anchor.LengthMillimeters,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

				svgElements.Add(lineHorBotSizeLengthThreadSecond);

				lineVerSizeDiamThreadSecond = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
										m_YOrigin + anchor.LengthMillimeters - m_ScaledSecondThreadLength,
										m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
										m_YOrigin + anchor.LengthMillimeters,
										Color.Black,
										0.5f,
										SvgLengthUnits.Pixels);

				svgElements.Add(lineVerSizeDiamThreadSecond);

				lineSerifTopSizeDiamThreadSecond = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + anchor.LengthMillimeters - m_ScaledSecondThreadLength,
									m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + anchor.LengthMillimeters - m_ScaledSecondThreadLength,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeDiamThreadSecond);

				lineSerifBotSizeDiamThreadSecond = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
										m_YOrigin + anchor.LengthMillimeters,
										m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
										m_YOrigin + anchor.LengthMillimeters,
										Color.Black,
										0.5f,
										SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifBotSizeDiamThreadSecond);

				svgElements.Add(GetSvgTextElement($"{anchor.ThreadSecondLengthMillimeters}",
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize - 2,
					m_YOrigin + anchor.LengthMillimeters - m_ScaledSecondThreadLength / 2 + 10,
					-90,
					SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of second thread

				// Draw part with second thread

				rectThreadSecondBodyAnchor = GetSvgRectElement(m_XOrigin,
					m_YOrigin + (anchor.LengthMillimeters - m_ScaledSecondThreadLength),
					anchor.ThreadDiameterMillimeters,
					m_ScaledSecondThreadLength,
					Color.Transparent,
					Color.Black,
					1.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(rectThreadSecondBodyAnchor);

				rectThreadSecondStepBodyAnchor = GetSvgRectElement(m_XOrigin + anchor.ThreadStepMillimeters / 2,
					m_YOrigin + (anchor.LengthMillimeters - m_ScaledSecondThreadLength),
					anchor.ThreadDiameterMillimeters - anchor.ThreadStepMillimeters,
					m_ScaledSecondThreadLength,
					Color.Transparent,
					Color.Black,
					1f,
					SvgLengthUnits.Pixels);

				svgElements.Add(rectThreadSecondStepBodyAnchor);

				// Draw part without thread

				rectBasicBodyAnchor = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + m_ScaledThreadLength,
				anchor.DiameterMillimeters,
				anchor.LengthMillimeters - m_ScaledThreadLength - m_ScaledSecondThreadLength,
				Color.Transparent,
				Color.Black,
				1,
				SvgLengthUnits.Pixels);

				svgElements.Add(rectBasicBodyAnchor);

				// Size of anchors's LengthMillimeters

				lineHorTopSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
							m_YOrigin,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthOfAnchor);

				lineHorBotSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
								m_YOrigin + anchor.LengthMillimeters,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);


				svgElements.Add(lineHorBotSizeLengthOfAnchor);


				lineVertSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin + anchor.LengthMillimeters,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineVertSizeLengthOfAnchor);

				var lineSerifTopSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeLengthOfAnchor);

				var lineSerifBotSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin + anchor.LengthMillimeters,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin + anchor.LengthMillimeters,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifBotSizeLengthOfAnchor);

				svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters}",
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2,
							m_YOrigin + anchor.LengthMillimeters / 2 + 10,
							-90,
							SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor

				lineAxialTopHalfOfAnchor = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin - outPartHorSize,
							m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.15f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineAxialTopHalfOfAnchor); // Make top axial line of anchor
			}
			else
			{
				rectBasicBodyAnchor = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + m_ScaledThreadLength,
				anchor.DiameterMillimeters,
				anchor.LengthMillimeters - m_ScaledThreadLength,
				Color.Transparent,
				Color.Black,
				1,
				SvgLengthUnits.Pixels);

				svgElements.Add(rectBasicBodyAnchor);

				// Size of anchors's LengthMillimeters

				lineHorTopSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
							m_YOrigin,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthOfAnchor);

				lineHorBotSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
								m_YOrigin + anchor.LengthMillimeters,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);


				svgElements.Add(lineHorBotSizeLengthOfAnchor);


				lineVertSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin + anchor.LengthMillimeters,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineVertSizeLengthOfAnchor);

				var lineSerifTopSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeLengthOfAnchor);

				var lineSerifBotSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin + anchor.LengthMillimeters,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin + anchor.LengthMillimeters,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifBotSizeLengthOfAnchor);

				svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters}",
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2,
							m_YOrigin + anchor.LengthMillimeters / 2 + 10,
							-90,
							SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor

				lineAxialTopHalfOfAnchor = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin - outPartHorSize,
							m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.15f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineAxialTopHalfOfAnchor); // Make top axial line of anchor
			}
		}
		else
		{
			// Draw basic part without thread and bend

			// Make top half basic part without thread and bend

			var pbHalfTopBasicBodyAnchor = new SvgPathBuilder();
			var pathHalfTopBasicBodyAnchor = new SvgPathElement();

			if (anchor.ThreadSecondLengthMillimeters > 0)
			{
				// Size of second thread's LengthMillimeters

				lineHorTopSizeLengthThreadSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - m_ScaledSecondThreadLength,
							m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - m_ScaledSecondThreadLength,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthThreadSecond);

				lineHorBotSizeLengthThreadSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
								m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

				svgElements.Add(lineHorBotSizeLengthThreadSecond);

				lineVerSizeDiamThreadSecond = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
										m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - m_ScaledSecondThreadLength,
										m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
										m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
										Color.Black,
										0.5f,
										SvgLengthUnits.Pixels);

				svgElements.Add(lineVerSizeDiamThreadSecond);

				lineSerifTopSizeDiamThreadSecond = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - m_ScaledSecondThreadLength,
									m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - m_ScaledSecondThreadLength,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeDiamThreadSecond);

				lineSerifBotSizeDiamThreadSecond = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
										m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
										m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
										m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
										Color.Black,
										0.5f,
										SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifBotSizeDiamThreadSecond);

				svgElements.Add(GetSvgTextElement($"{anchor.ThreadSecondLengthMillimeters}",
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize - 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - m_ScaledSecondThreadLength / 2 + 10,
					-90,
					SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of second thread

				// Draw part with second thread

				rectThreadSecondBodyAnchor = GetSvgRectElement(m_XOrigin,
					m_YOrigin + (ANCHOR_LENGTH_MAX + m_ScaledThreadLength - m_ScaledSecondThreadLength),
					anchor.ThreadDiameterMillimeters,
					m_ScaledSecondThreadLength,
					Color.Transparent,
					Color.Black,
					1.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(rectThreadSecondBodyAnchor);

				rectThreadSecondStepBodyAnchor = GetSvgRectElement(m_XOrigin + anchor.ThreadStepMillimeters / 2,
					m_YOrigin + (ANCHOR_LENGTH_MAX + m_ScaledThreadLength - m_ScaledSecondThreadLength),
					anchor.ThreadDiameterMillimeters - anchor.ThreadStepMillimeters,
					m_ScaledSecondThreadLength,
					Color.Transparent,
					Color.Black,
					1f,
					SvgLengthUnits.Pixels);

				svgElements.Add(rectThreadSecondStepBodyAnchor);

				pbHalfTopBasicBodyAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
				pbHalfTopBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength);
				pbHalfTopBasicBodyAnchor.AddHorizontalLineTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2);
				pbHalfTopBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

				pathHalfTopBasicBodyAnchor.PathData = pbHalfTopBasicBodyAnchor.ToPathData();
				pathHalfTopBasicBodyAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfTopBasicBodyAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfTopBasicBodyAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfTopBasicBodyAnchor);

				// Make gap Top Line

				var pbgapTop = new SvgPathBuilder();
				var pathgapTop = new SvgPathElement();

				pbgapTop.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
						m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
				pbgapTop.AddCurveTo(false, m_XOrigin - anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap) - 5,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

				pathgapTop.PathData = pbgapTop.ToPathData();
				pathgapTop.Fill = new SvgPaint(Color.Transparent);
				pathgapTop.Stroke = new SvgPaint(Color.Black);
				pathgapTop.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapTop);

				SvgLineElement lineSerifBotSizeLengthOfAnchor;

				lineAxialTopHalfOfAnchor = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin - outPartHorSize,
							m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
							Color.Black,
							0.15f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineAxialTopHalfOfAnchor); // Make top axial line of anchor

				// Make gap Bot Line

				var pbgapBot = new SvgPathBuilder();
				var pathgapBot = new SvgPathElement();

				pbgapBot.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbgapBot.AddCurveTo(false, m_XOrigin - anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2 - 5,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathgapBot.PathData = pbgapBot.ToPathData();
				pathgapBot.Fill = new SvgPaint(Color.Transparent);
				pathgapBot.Stroke = new SvgPaint(Color.Black);
				pathgapBot.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapBot);

				lineAxialBotHalfOfAnchor = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
						Color.Black,
						0.15f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineAxialBotHalfOfAnchor); // Make bot axial line of anchor

				// Size of anchors's LengthMillimeters

				lineHorTopSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
							m_YOrigin,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthOfAnchor);


				var pbHalfBotBasicBodyAnchor = new SvgPathBuilder();
				var pathHalfBotBasicBodyAnchor = new SvgPathElement();

				// Make bottom half basic part without thread and bend

				pbHalfBotBasicBodyAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbHalfBotBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - m_ScaledSecondThreadLength);
				pbHalfBotBasicBodyAnchor.AddHorizontalLineTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2);
				pbHalfBotBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathHalfBotBasicBodyAnchor.PathData = pbHalfBotBasicBodyAnchor.ToPathData();
				pathHalfBotBasicBodyAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfBotBasicBodyAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfBotBasicBodyAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfBotBasicBodyAnchor);

				// Make size anchors LengthMillimeters

				lineHorBotSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

				lineSerifBotSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				lineVertSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters}",
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2,
						m_YOrigin + (m_ScaledThreadLength + ANCHOR_LENGTH_MAX) / 2 + 10,
						-90,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor



				svgElements.Add(lineHorBotSizeLengthOfAnchor);

				svgElements.Add(lineSerifBotSizeLengthOfAnchor);

				svgElements.Add(lineVertSizeLengthOfAnchor);

				var lineSerifTopSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeLengthOfAnchor);
			}
			else
			{
				pbHalfTopBasicBodyAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
				pbHalfTopBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength);
				pbHalfTopBasicBodyAnchor.AddHorizontalLineTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2);
				pbHalfTopBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

				pathHalfTopBasicBodyAnchor.PathData = pbHalfTopBasicBodyAnchor.ToPathData();
				pathHalfTopBasicBodyAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfTopBasicBodyAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfTopBasicBodyAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfTopBasicBodyAnchor);

				// Make gap Top Line

				var pbgapTop = new SvgPathBuilder();
				var pathgapTop = new SvgPathElement();
				float halfDiam;

				if (anchor.ThreadLengthMillimeters > 0)
					halfDiam = anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters / 2;
				else
					halfDiam = 0;

				pbgapTop.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
						m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
				pbgapTop.AddCurveTo(false, m_XOrigin + halfDiam,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap) - 5,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

				pathgapTop.PathData = pbgapTop.ToPathData();
				pathgapTop.Fill = new SvgPaint(Color.Transparent);
				pathgapTop.Stroke = new SvgPaint(Color.Black);
				pathgapTop.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapTop);

				SvgLineElement lineSerifBotSizeLengthOfAnchor;

				lineAxialTopHalfOfAnchor = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin - outPartHorSize,
							m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
							Color.Black,
							0.15f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineAxialTopHalfOfAnchor); // Make top axial line of anchor

				// Make gap Bot Line

				var pbgapBot = new SvgPathBuilder();
				var pathgapBot = new SvgPathElement();

				pbgapBot.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbgapBot.AddCurveTo(false, m_XOrigin + halfDiam,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2 - 5,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathgapBot.PathData = pbgapBot.ToPathData();
				pathgapBot.Fill = new SvgPaint(Color.Transparent);
				pathgapBot.Stroke = new SvgPaint(Color.Black);
				pathgapBot.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapBot);

				lineAxialBotHalfOfAnchor = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
						Color.Black,
						0.15f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineAxialBotHalfOfAnchor); // Make bot axial line of anchor

				// Size of anchors's LengthMillimeters

				lineHorTopSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
							m_YOrigin,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthOfAnchor);


				var pbHalfBotBasicBodyAnchor = new SvgPathBuilder();
				var pathHalfBotBasicBodyAnchor = new SvgPathElement();

				// Make bottom half basic part without thread and bend

				pbHalfBotBasicBodyAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbHalfBotBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX);
				pbHalfBotBasicBodyAnchor.AddHorizontalLineTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2);
				pbHalfBotBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathHalfBotBasicBodyAnchor.PathData = pbHalfBotBasicBodyAnchor.ToPathData();
				pathHalfBotBasicBodyAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfBotBasicBodyAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfBotBasicBodyAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfBotBasicBodyAnchor);

				// Make size anchors LengthMillimeters

				lineHorBotSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

				lineSerifBotSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				lineVertSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters}",
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2,
						m_YOrigin + (m_ScaledThreadLength + ANCHOR_LENGTH_MAX) / 2 + 10,
						-90,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor



				svgElements.Add(lineHorBotSizeLengthOfAnchor);

				svgElements.Add(lineSerifBotSizeLengthOfAnchor);

				svgElements.Add(lineVertSizeLengthOfAnchor);

				var lineSerifTopSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
							m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeLengthOfAnchor);
			}
		}

		// GetDescriptionAnchor(anchor, paramsCanvas, svgElements);

		for (int i = 0; i < svgElements.Count; i++)
			svgDoc.RootSvg.Children.Insert(i, svgElements[i]);

		SvgViewBox view = new()
		{
			MinX = 0,
			MinY = 0,
			Width = VIEW_WIDTH_PIXELS,
			Height = VIEW_HEIGHT_PIXELS
		};

		svgDoc.RootSvg.ViewBox = view;

		StringBuilder stringBuilder = new();
		svgDoc.Save(stringBuilder);
		string xml = stringBuilder.ToString();
		string svgElem = xml.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
		anchor.SvgElement = svgElem;
	}

	public void GetSvgBendAnchor(Anchor anchor)
	{
		string iconDiameter = anchor.Material.TypeId == 1 ? "Арм" : "⌀";

		if (anchor.BendLengthMillimeters <= ANCHOR_BEND_LENGTH_MAX)
			m_XOrigin += anchor.BendLengthMillimeters; // X origin
		else
			m_XOrigin += ANCHOR_BEND_LENGTH_MAX; // X origin

		GetScaledLength(anchor.ThreadLengthMillimeters, anchor.ThreadSecondLengthMillimeters); // Scaling ThreadLengthMillimeters

		int gap = 20; // Gap in out of max LengthMillimeters  of anchor
		int outPartHorSize = 45; // LengthMillimeters output part of horizontal size
		int outPartRadSize = 45; // LengthMillimeters of shelf of radius size

		var svgDoc = new GcSvgDocument();
		svgDoc.RootSvg.Width = new SvgLength(WIDTH_PIXELS, SvgLengthUnits.Pixels);
		svgDoc.RootSvg.Height = new SvgLength(HEIGHT_PIXELS, SvgLengthUnits.Pixels);

		List<SvgElement> svgElements = new(); // Make list to fill with objects SvgRectElement

		if (anchor.ThreadLengthMillimeters > 0)
		{

			// Draw part with thread

			var rectThreadBodyAnchor = GetSvgRectElement(m_XOrigin,
				m_YOrigin,
				anchor.ThreadDiameterMillimeters,
				m_ScaledThreadLength,
				Color.Transparent,
				Color.Black,
				1.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(rectThreadBodyAnchor);

			var rectThreadStepBodyAnchor = GetSvgRectElement(m_XOrigin + anchor.ThreadStepMillimeters / 2,
				m_YOrigin,
				anchor.ThreadDiameterMillimeters - anchor.ThreadStepMillimeters,
				m_ScaledThreadLength,
				Color.Transparent,
				Color.Black,
				1f,
				SvgLengthUnits.Pixels);

			svgElements.Add(rectThreadStepBodyAnchor);

			// Draw sizes of part with thread

			// Size of thread's diametr

			var lineVertLeftSizeDiamThread = GetSvgLineElement(m_XOrigin,
				m_YOrigin,
				m_XOrigin,
				m_YOrigin - outPartHorSize,
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(lineVertLeftSizeDiamThread);

			var lineVertRightSizeDiamThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
					m_YOrigin,
					m_XOrigin + anchor.ThreadDiameterMillimeters,
					m_YOrigin - outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineVertRightSizeDiamThread);

			var lineHorSizeDiamThread = GetSvgLineElement(m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						m_XOrigin + anchor.ThreadDiameterMillimeters + 105,
						m_YOrigin - (outPartHorSize - 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorSizeDiamThread);

			var lineSerifLeftSizeDiamThread = GetSerif(m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifLeftSizeDiamThread);

			var lineSerifRightSizeDiamThread = GetSerif(m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin - (outPartHorSize - 5),
							m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin - (outPartHorSize - 5),
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifRightSizeDiamThread);


			svgElements.Add(GetSvgTextElement($"М{anchor.ThreadDiameterMillimeters}x{anchor.ThreadStepMillimeters}",
							m_XOrigin + anchor.ThreadDiameterMillimeters + 8,
							m_YOrigin - (outPartHorSize - 3),
							0,
							SvgLengthUnits.Pixels)); // Make text of size's value diametr of thread

			// Size of thread's LengthMillimeters

			var lineHorTopSizeLengthThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
						m_YOrigin,
						m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthThread);

			var lineHorBotSizeLengthThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength,
							m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
							m_YOrigin + m_ScaledThreadLength,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineHorBotSizeLengthThread);

			var lineVerSizeDiamThread = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin,
									m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

			svgElements.Add(lineVerSizeDiamThread);

			var lineSerifTopSizeDiamThread = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
								m_YOrigin,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
								m_YOrigin,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeDiamThread);

			var lineSerifBotSizeDiamThread = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeDiamThread);

			svgElements.Add(GetSvgTextElement($"{anchor.ThreadLengthMillimeters}",
				m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize - 2,
				m_YOrigin + m_ScaledThreadLength / 2 + 10,
				-90,
				SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of thread
		}

		// Size of anchors's diametr

		var lineHorSizeDiamAnchor = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2 + 55,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineHorSizeDiamAnchor);

		var lineSerifLeftSizeDiamAnchor = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineSerifLeftSizeDiamAnchor);

		var lineSerifRightSizeDiamAnchor = GetSerif(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineSerifRightSizeDiamAnchor);

		svgElements.Add(GetSvgTextElement($"{iconDiameter}{anchor.DiameterMillimeters}",
			m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2 + 5,
			m_YOrigin + m_ScaledThreadLength + outPartHorSize - 2,
			0,
			SvgLengthUnits.Pixels)); // Make text of size's value diametr of anchor

		// Make object basic part without thread and bend

		SvgRectElement rectBasicBodyAnchor;

		// Make objects of sizes anchor's LengthMillimeters

		SvgLineElement lineHorTopSizeLengthOfAnchor;
		SvgLineElement lineHorBotSizeLengthOfAnchor;
		SvgLineElement lineVertSizeLengthOfAnchor;
		SvgLineElement lineSerifTopSizeLengthOfAnchor;

		SvgLineElement lineHorTopSizeLengthOfAnchorWithoutRadius;
		SvgLineElement lineHorBotSizeLengthOfAnchorWithoutRadius;
		SvgLineElement lineVertSizeLengthOfAnchorWithoutRadius;

		SvgPathBuilder pbAxialTopHalfOfAnchor = new();
		SvgPathElement pathAxialTopHalfOfAnchor = new();

		SvgPathBuilder pbAxialBotHalfOfAnchor = new();
		SvgPathElement pathAxialBotHalfOfAnchor = new();

		SvgLineElement lineAxialToptHalfOfAnchor;
		SvgLineElement lineAxialBotLeftHalfOfAnchor;

		// Make object bending part without radius

		SvgRectElement rectBendAnchor;

		// Make object bending part with radius

		var pbRadiusBend = new SvgPathBuilder();
		var pathRadiusBend = new SvgPathElement();

		if (anchor.LengthMillimeters <= ANCHOR_LENGTH_MAX)
		{
			
				// Draw basic part without thread and bend

				rectBasicBodyAnchor = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength,
					anchor.DiameterMillimeters,
					anchor.LengthMillimeters - (m_ScaledThreadLength + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
					Color.Transparent,
					Color.Black,
					1.5f,
					SvgLengthUnits.Pixels);

				if (anchor.BendLengthMillimeters <= ANCHOR_BEND_LENGTH_MAX)
				{
					// Draw bending part without radius

					rectBendAnchor = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - (anchor.BendLengthMillimeters - anchor.DiameterMillimeters),
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters,
						anchor.BendLengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						anchor.DiameterMillimeters,
						Color.Transparent,
						Color.Black,
						1.5f,
						SvgLengthUnits.Pixels);

					svgElements.Add(rectBendAnchor);

					// Size of bending part

					var lineVertLeftSizeBendPart = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters,
							m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineVertLeftSizeBendPart);

					var lineVertRightSizeBendPart = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineVertRightSizeBendPart);

					var lineHorSizeBendPart = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineHorSizeBendPart);

					var lineSerifLeftSizeBendPart = GetSerif(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineSerifLeftSizeBendPart);

					var lineSerifRightSizeBendPart = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineSerifRightSizeBendPart);

					svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters}",
							m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - anchor.BendLengthMillimeters) / 2 - 10,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize - 2 + outPartHorSize,
							0,
							SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of thread


				// Size of bending part without radius

				var lineVertLeftSizeBendPartWithouRadius = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPartWithouRadius);

				var lineVertRightSizeBendPartWithouRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPartWithouRadius);

				var lineHorSizeBendPartWithouRadius = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPartWithouRadius);

				var lineSerifLeftSizeBendPartWithouRadius = GetSerif(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPartWithouRadius);

				var lineSerifRightSizeBendPartWithouRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPartWithouRadius);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters}",
						m_XOrigin + (anchor.ThreadDiameterMillimeters - anchor.BendLengthMillimeters - anchor.BendRadiusMillimeters) / 2 - 10,
						m_YOrigin + anchor.LengthMillimeters + outPartHorSize - 2,
						0,
						SvgLengthUnits.Pixels)); // Make text of size's value of bending part without radius

				pbAxialTopHalfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin - outPartHorSize);
					pbAxialTopHalfOfAnchor.AddVerticalLineTo(false, m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters));
					pbAxialTopHalfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2);
					pbAxialTopHalfOfAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.BendLengthMillimeters + outPartHorSize));

					pathAxialTopHalfOfAnchor.PathData = pbAxialTopHalfOfAnchor.ToPathData();
					pathAxialTopHalfOfAnchor.Fill = new SvgPaint(Color.Transparent);
					pathAxialTopHalfOfAnchor.Stroke = new SvgPaint(Color.Black);
					pathAxialTopHalfOfAnchor.StrokeWidth = new SvgLength(0.15f);

					svgElements.Add(pathAxialTopHalfOfAnchor); // Make top axial line of anchor
				}
				else
				{
					// Draw bending part without radius

					// Make right half bending part without radius

					var pbHalfRightBendPartAnchor = new SvgPathBuilder();
					var pathHalfRightBendPartAnchor = new SvgPathElement();

					pbHalfRightBendPartAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
					pbHalfRightBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters);
					pbHalfRightBendPartAnchor.AddVerticalLineTo(false, m_YOrigin + anchor.LengthMillimeters);
					pbHalfRightBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap);

					pathHalfRightBendPartAnchor.PathData = pbHalfRightBendPartAnchor.ToPathData();
					pathHalfRightBendPartAnchor.Fill = new SvgPaint(Color.Transparent);
					pathHalfRightBendPartAnchor.Stroke = new SvgPaint(Color.Black);
					pathHalfRightBendPartAnchor.StrokeWidth = new SvgLength(1.5f);

					svgElements.Add(pathHalfRightBendPartAnchor);

					// Make left half bending part without radius

					var pbHalfLeftBendPartAnchor = new SvgPathBuilder();
					var pathHalfLeftBendPartAnchor = new SvgPathElement();

					pbHalfLeftBendPartAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
					pbHalfLeftBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX);
					pbHalfLeftBendPartAnchor.AddVerticalLineTo(false, m_YOrigin + anchor.LengthMillimeters);
					pbHalfLeftBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2);

					pathHalfLeftBendPartAnchor.PathData = pbHalfLeftBendPartAnchor.ToPathData();
					pathHalfLeftBendPartAnchor.Fill = new SvgPaint(Color.Transparent);
					pathHalfLeftBendPartAnchor.Stroke = new SvgPaint(Color.Black);
					pathHalfLeftBendPartAnchor.StrokeWidth = new SvgLength(1.5f);

					svgElements.Add(pathHalfLeftBendPartAnchor);

					// Make gap Right Line

					var pbgapRight = new SvgPathBuilder();
					var pathgapRight = new SvgPathElement();

					pbgapRight.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
							m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
					pbgapRight.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap - 5,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + anchor.LengthMillimeters);

					pathgapRight.PathData = pbgapRight.ToPathData();
					pathgapRight.Fill = new SvgPaint(Color.Transparent);
					pathgapRight.Stroke = new SvgPaint(Color.Black);
					pathgapRight.StrokeWidth = new SvgLength(0.5f);

					svgElements.Add(pathgapRight);

					// Make gap Left Line

					var pbgapLeft = new SvgPathBuilder();
					var pathgapLeft = new SvgPathElement();

					pbgapLeft.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
					pbgapLeft.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 - 5,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
						m_YOrigin + anchor.LengthMillimeters);

					pathgapLeft.PathData = pbgapLeft.ToPathData();
					pathgapLeft.Fill = new SvgPaint(Color.Transparent);
					pathgapLeft.Stroke = new SvgPaint(Color.Black);
					pathgapLeft.StrokeWidth = new SvgLength(0.5f);

					svgElements.Add(pathgapLeft);

					// Size of bending part

					var lineVertLeftSizeBendPart = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineVertLeftSizeBendPart);

					var lineVertRightSizeBendPart = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineVertRightSizeBendPart);

					var lineHorSizeBendPart = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineHorSizeBendPart);

					var lineSerifLeftSizeBendPart = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineSerifLeftSizeBendPart);

					var lineSerifRightSizeBendPart = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineSerifRightSizeBendPart);

					svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters}",
							m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - ANCHOR_BEND_LENGTH_MAX) / 2 - 10,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize - 2 + outPartHorSize,
							0,
							SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of thread

				// Size of bending part without radius

				var lineVertLeftSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPartWithoutRadius);

				var lineVertRightSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) ,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPartWithoutRadius);

				var lineHorSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPartWithoutRadius);

				var lineSerifLeftSizeBendPartWithoutRadius = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPartWithoutRadius);

				var lineSerifRightSizeBendPartWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPartWithoutRadius);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters}",
						m_XOrigin + ( anchor.ThreadDiameterMillimeters - ANCHOR_BEND_LENGTH_MAX - anchor.BendRadiusMillimeters ) / 2 - 10,
						m_YOrigin + anchor.LengthMillimeters + outPartHorSize - 2,
						0,
						SvgLengthUnits.Pixels)); // Make text of size's value of bending part without radius

				pbAxialTopHalfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin - outPartHorSize);
					pbAxialTopHalfOfAnchor.AddVerticalLineTo(false, m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters));
					pbAxialTopHalfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2);
					pbAxialTopHalfOfAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap);

					pathAxialTopHalfOfAnchor.PathData = pbAxialTopHalfOfAnchor.ToPathData();
					pathAxialTopHalfOfAnchor.Fill = new SvgPaint(Color.Transparent);
					pathAxialTopHalfOfAnchor.Stroke = new SvgPaint(Color.Black);
					pathAxialTopHalfOfAnchor.StrokeWidth = new SvgLength(0.15f);

					svgElements.Add(pathAxialTopHalfOfAnchor); // Make top axial line of anchor

					lineAxialBotLeftHalfOfAnchor = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
							m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters/2,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX - outPartHorSize,
							m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters/2,
							Color.Black,
							0.15f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineAxialBotLeftHalfOfAnchor); // Make bot left part of axial line of anchor
				}

				// Draw bending part with radius

				pbRadiusBend.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters));
				pbRadiusBend.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
				pbRadiusBend.AddVerticalLineTo(false, m_YOrigin + anchor.LengthMillimeters);
				pbRadiusBend.AddCurveTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
					m_YOrigin + anchor.LengthMillimeters,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
					m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
					m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters));


				pathRadiusBend.PathData = pbRadiusBend.ToPathData();
				pathRadiusBend.Fill = new SvgPaint(Color.Transparent);
				pathRadiusBend.Stroke = new SvgPaint(Color.Black);
				pathRadiusBend.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathRadiusBend);

				// Size of radius

				var lineInclinSizeRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineInclinSizeRadius);

				var lineHorSizeRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - outPartRadSize,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeRadius);

				var lineSerifSizeRadius = GetSerifRad(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifSizeRadius);

				svgElements.Add(GetSvgTextElement($"R{anchor.BendRadiusMillimeters}",
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - outPartRadSize,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters - 2,
					0,
					SvgLengthUnits.Pixels)); // Make text of size's value radius of anchor

			svgElements.Add(rectBasicBodyAnchor);

			// Size of anchors's LengthMillimeters

			lineHorTopSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize + outPartRadSize),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthOfAnchor);
		
			lineHorBotSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								m_YOrigin + anchor.LengthMillimeters,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize + outPartRadSize),
								m_YOrigin + anchor.LengthMillimeters,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);
				
			svgElements.Add(lineHorBotSizeLengthOfAnchor);

			lineVertSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin + anchor.LengthMillimeters,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineVertSizeLengthOfAnchor);

			lineSerifTopSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeLengthOfAnchor);

			var lineSerifBotSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin + anchor.LengthMillimeters,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeLengthOfAnchor);

			svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters}",
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2 + outPartRadSize,
						m_YOrigin + anchor.LengthMillimeters / 2 + 10,
						-90,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor

			// Size of anchors's LengthMillimeters without radius

			lineHorTopSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthOfAnchorWithoutRadius);

			lineHorBotSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
								m_YOrigin + anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
								m_YOrigin + anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

			svgElements.Add(lineHorBotSizeLengthOfAnchorWithoutRadius);

			lineVertSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineVertSizeLengthOfAnchorWithoutRadius);

			var lineSerifTopSizeLengthOfAnchorWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeLengthOfAnchorWithoutRadius);

			var lineSerifBotSizeLengthOfAnchorWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeLengthOfAnchorWithoutRadius);

			svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters}",
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2,
						m_YOrigin + (anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters) / 2 + 10,
						-90,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor without radius
		}
		else
		{
			// Draw basic part without thread and bend

			// Make top half basic part without thread and bend

			var pbHalfTopBasicBodyAnchor = new SvgPathBuilder();
			var pathHalfTopBasicBodyAnchor = new SvgPathElement();

			pbHalfTopBasicBodyAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
			pbHalfTopBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength);
			pbHalfTopBasicBodyAnchor.AddHorizontalLineTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2);
			pbHalfTopBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

			pathHalfTopBasicBodyAnchor.PathData = pbHalfTopBasicBodyAnchor.ToPathData();
			pathHalfTopBasicBodyAnchor.Fill = new SvgPaint(Color.Transparent);
			pathHalfTopBasicBodyAnchor.Stroke = new SvgPaint(Color.Black);
			pathHalfTopBasicBodyAnchor.StrokeWidth = new SvgLength(1.5f);

			svgElements.Add(pathHalfTopBasicBodyAnchor);

			// Make gap Top Line

			var pbgapTop = new SvgPathBuilder();
			var pathgapTop = new SvgPathElement();
			float halfDiam;

			if (anchor.ThreadLengthMillimeters > 0)
				halfDiam = anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters / 2;
			else
				halfDiam = 0;

			pbgapTop.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
			pbgapTop.AddCurveTo(false, m_XOrigin + halfDiam,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap) - 5,
				m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
				m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

			pathgapTop.PathData = pbgapTop.ToPathData();
			pathgapTop.Fill = new SvgPaint(Color.Transparent);
			pathgapTop.Stroke = new SvgPaint(Color.Black);
			pathgapTop.StrokeWidth = new SvgLength(0.5f);

			svgElements.Add(pathgapTop);

			SvgLineElement lineSerifBotSizeLengthOfAnchor;

			// Make gap Bot Line

			var pbgapBot = new SvgPathBuilder();
			var pathgapBot = new SvgPathElement();

			pbgapBot.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
			pbgapBot.AddCurveTo(false, m_XOrigin + halfDiam,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2 - 5,
				m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
				m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

			pathgapBot.PathData = pbgapBot.ToPathData();
			pathgapBot.Fill = new SvgPaint(Color.Transparent);
			pathgapBot.Stroke = new SvgPaint(Color.Black);
			pathgapBot.StrokeWidth = new SvgLength(0.5f);

			svgElements.Add(pathgapBot);

			// Size of anchors's LengthMillimeters

			lineHorTopSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize) + outPartRadSize,
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthOfAnchor);

				lineHorBotSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize) + outPartRadSize,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);


					lineSerifTopSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
							m_YOrigin,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
							m_YOrigin,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			lineSerifBotSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				lineVertSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartRadSize,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters}",
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2 + outPartRadSize,
						m_YOrigin + (m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters) / 2 + 10,
						-90,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor

			// Size of anchors's LengthMillimeters without radius

			lineHorTopSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthOfAnchorWithoutRadius);

			lineHorBotSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineHorBotSizeLengthOfAnchorWithoutRadius);

			lineVertSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineVertSizeLengthOfAnchorWithoutRadius);

			var lineSerifTopSizeLengthOfAnchorWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeLengthOfAnchorWithoutRadius);

			var lineSerifBotSizeLengthOfAnchorWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeLengthOfAnchorWithoutRadius);

			svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters}",
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2,
					m_YOrigin + (m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters) / 2 + 10,
					-90,
					SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor withou radius


			var pbHalfBotBasicBodyAnchor = new SvgPathBuilder();
			var pathHalfBotBasicBodyAnchor = new SvgPathElement();

			
				// Make bottom half basic part without thread and bend

				pbHalfBotBasicBodyAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbHalfBotBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters);
				pbHalfBotBasicBodyAnchor.AddHorizontalLineTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2);
				pbHalfBotBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathHalfBotBasicBodyAnchor.PathData = pbHalfBotBasicBodyAnchor.ToPathData();
				pathHalfBotBasicBodyAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfBotBasicBodyAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfBotBasicBodyAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfBotBasicBodyAnchor);

				// Draw bending part with radius

				pbRadiusBend.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters);
				pbRadiusBend.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
				pbRadiusBend.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX);
				pbRadiusBend.AddCurveTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
					m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters);

				pathRadiusBend.PathData = pbRadiusBend.ToPathData();
				pathRadiusBend.Fill = new SvgPaint(Color.Transparent);
				pathRadiusBend.Stroke = new SvgPaint(Color.Black);
				pathRadiusBend.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathRadiusBend);

				lineAxialToptHalfOfAnchor = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
									m_YOrigin - outPartHorSize,
									m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
									m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
									Color.Black,
									0.15f,
									SvgLengthUnits.Pixels);

				svgElements.Add(lineAxialToptHalfOfAnchor); // Make top part of axial line of anchor

			if (anchor.BendLengthMillimeters <= ANCHOR_BEND_LENGTH_MAX)
				{
					// Draw bending part without radius

					rectBendAnchor = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - (anchor.BendLengthMillimeters - anchor.DiameterMillimeters),
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters,
						anchor.BendLengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						anchor.DiameterMillimeters,
						Color.Transparent,
						Color.Black,
						1.5f,
						SvgLengthUnits.Pixels);

					svgElements.Add(rectBendAnchor);

					// Size of bending part

					var lineVertLeftSizeBendPart = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
							m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + (outPartHorSize + 5) + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineVertLeftSizeBendPart);

					var lineVertRightSizeBendPart = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + (outPartHorSize + 5) + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineVertRightSizeBendPart);

					var lineHorSizeBendPart = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineHorSizeBendPart);

					var lineSerifLeftSizeBendPart = GetSerif(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
								m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineSerifLeftSizeBendPart);

					var lineSerifRightSizeBendPart = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineSerifRightSizeBendPart);

					svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters}",
							m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - anchor.BendLengthMillimeters) / 2 - 10,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize - 2 + outPartHorSize,
							0,
							SvgLengthUnits.Pixels)); // Make text of size's value of bending part

				// Size of bending part without radius

				var lineVertLeftSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPartWithoutRadius);

				var lineVertRightSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPartWithoutRadius);

				var lineHorSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPartWithoutRadius);

				var lineSerifLeftSizeBendPartWithoutRadius = GetSerif(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPartWithoutRadius);

				var lineSerifRightSizeBendPartWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPartWithoutRadius);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - anchor.BendLengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters) / 2 - 10,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize - 2,
						0,
						SvgLengthUnits.Pixels)); // Make text of size's of bending part without radius

				pbAxialBotHalfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
					pbAxialBotHalfOfAnchor.AddVerticalLineTo(false, m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters));
					pbAxialBotHalfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2);
					pbAxialBotHalfOfAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.BendLengthMillimeters + outPartHorSize));
						
						pathAxialBotHalfOfAnchor.PathData = pbAxialBotHalfOfAnchor.ToPathData();
					pathAxialBotHalfOfAnchor.Fill = new SvgPaint(Color.Transparent);
					pathAxialBotHalfOfAnchor.Stroke = new SvgPaint(Color.Black);
					pathAxialBotHalfOfAnchor.StrokeWidth = new SvgLength(0.15f);

					svgElements.Add(pathAxialBotHalfOfAnchor); // Make bot axial line of anchor
				}
			else
				{
					// Draw bending part without radius

					// Make right half bending part without radius

					var pbHalfRightBendPartAnchor = new SvgPathBuilder();
					var pathHalfRightBendPartAnchor = new SvgPathElement();

					pbHalfRightBendPartAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
					pbHalfRightBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters);
					pbHalfRightBendPartAnchor.AddVerticalLineTo(false, m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength);
					pbHalfRightBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap);

					pathHalfRightBendPartAnchor.PathData = pbHalfRightBendPartAnchor.ToPathData();
					pathHalfRightBendPartAnchor.Fill = new SvgPaint(Color.Transparent);
					pathHalfRightBendPartAnchor.Stroke = new SvgPaint(Color.Black);
					pathHalfRightBendPartAnchor.StrokeWidth = new SvgLength(1.5f);

					svgElements.Add(pathHalfRightBendPartAnchor);

					// Make left half bending part without radius

					var pbHalfLeftBendPartAnchor = new SvgPathBuilder();
					var pathHalfLeftBendPartAnchor = new SvgPathElement();

					pbHalfLeftBendPartAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
					pbHalfLeftBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX);
					pbHalfLeftBendPartAnchor.AddVerticalLineTo(false, m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength);
					pbHalfLeftBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2);

					pathHalfLeftBendPartAnchor.PathData = pbHalfLeftBendPartAnchor.ToPathData();
					pathHalfLeftBendPartAnchor.Fill = new SvgPaint(Color.Transparent);
					pathHalfLeftBendPartAnchor.Stroke = new SvgPaint(Color.Black);
					pathHalfLeftBendPartAnchor.StrokeWidth = new SvgLength(1.5f);

					svgElements.Add(pathHalfLeftBendPartAnchor);

					// Make gap Right Line

					var pbgapRight = new SvgPathBuilder();
					var pathgapRight = new SvgPathElement();

					pbgapRight.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
					pbgapRight.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap - 5,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength);

					pathgapRight.PathData = pbgapRight.ToPathData();
					pathgapRight.Fill = new SvgPaint(Color.Transparent);
					pathgapRight.Stroke = new SvgPaint(Color.Black);
					pathgapRight.StrokeWidth = new SvgLength(0.5f);

					svgElements.Add(pathgapRight);

					// Make gap Left Line

					var pbgapLeft = new SvgPathBuilder();
					var pathgapLeft = new SvgPathElement();

					pbgapLeft.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
					pbgapLeft.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 - 5,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength);

					pathgapLeft.PathData = pbgapLeft.ToPathData();
					pathgapLeft.Fill = new SvgPaint(Color.Transparent);
					pathgapLeft.Stroke = new SvgPaint(Color.Black);
					pathgapLeft.StrokeWidth = new SvgLength(0.5f);

					svgElements.Add(pathgapLeft);

					// Size of bending part

					var lineVertLeftSizeBendPart = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + (outPartHorSize + 5) + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineVertLeftSizeBendPart);

					var lineVertRightSizeBendPart = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + (outPartHorSize + 5) + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

					svgElements.Add(lineVertRightSizeBendPart);

					var lineHorSizeBendPart = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineHorSizeBendPart);

					var lineSerifLeftSizeBendPart = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
								m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineSerifLeftSizeBendPart);

					var lineSerifRightSizeBendPart = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

					svgElements.Add(lineSerifRightSizeBendPart);

					svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters}",
							m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - ANCHOR_BEND_LENGTH_MAX) / 2 - 10,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize - 2 + outPartHorSize,
							0,
							SvgLengthUnits.Pixels)); // Make text of size's value bending part

				// Size of bending part without radius

				var lineVertLeftSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPartWithoutRadius);

				var lineVertRightSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPartWithoutRadius);

				var lineHorSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPartWithoutRadius);

				var lineSerifLeftSizeBendPartWithoutRadius = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPartWithoutRadius);

				var lineSerifRightSizeBendPartWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPartWithoutRadius);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - ANCHOR_BEND_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters) / 2 - 10,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize - 2,
						0,
						SvgLengthUnits.Pixels)); // Make text of size's value without bending part

				pbAxialBotHalfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
					pbAxialBotHalfOfAnchor.AddVerticalLineTo(false, m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters));
					pbAxialBotHalfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2);
					pbAxialBotHalfOfAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap);

					pathAxialBotHalfOfAnchor.PathData = pbAxialBotHalfOfAnchor.ToPathData();
					pathAxialBotHalfOfAnchor.Fill = new SvgPaint(Color.Transparent);
					pathAxialBotHalfOfAnchor.Stroke = new SvgPaint(Color.Black);
					pathAxialBotHalfOfAnchor.StrokeWidth = new SvgLength(0.15f);

					svgElements.Add(pathAxialBotHalfOfAnchor); // Make bot axial line of anchor


					lineAxialBotLeftHalfOfAnchor = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
											m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
											m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX - outPartHorSize,
											m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
											Color.Black,
											0.15f,
											SvgLengthUnits.Pixels);

					svgElements.Add(lineAxialBotLeftHalfOfAnchor); // Make bot part of axial line of anchor
				}

				// Size of radius

				var lineInclinSizeRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2) - anchor.DiameterMillimeters,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineInclinSizeRadius);

				var lineHorSizeRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - outPartRadSize,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeRadius);

				var lineSerifSizeRadius = GetSerifRad(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2) - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2) - anchor.DiameterMillimeters,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifSizeRadius);

				svgElements.Add(GetSvgTextElement($"R{anchor.BendRadiusMillimeters}",
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - outPartRadSize,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) - 2,
					0,
					SvgLengthUnits.Pixels)); // Make text of size's value radius of anchor

			svgElements.Add(lineHorBotSizeLengthOfAnchor);

			svgElements.Add(lineVertSizeLengthOfAnchor);

			svgElements.Add(lineSerifBotSizeLengthOfAnchor);

			svgElements.Add(lineSerifTopSizeLengthOfAnchor);
		}

		// GetDescriptionAnchor(anchor, paramsCanvas, svgElements);

		for (int i = 0; i < svgElements.Count; i++)
			svgDoc.RootSvg.Children.Insert(i, svgElements[i]);

		SvgViewBox view = new();
		view.MinX = 0;
		view.MinY = 0;
		view.Width = VIEW_WIDTH_PIXELS;
		view.Height = VIEW_HEIGHT_PIXELS;

		svgDoc.RootSvg.ViewBox = view;

		StringBuilder stringBuilder = new();
		svgDoc.Save(stringBuilder);
		string xml = stringBuilder.ToString();
		string svgElem = xml.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
		anchor.SvgElement = svgElem;
	}

	public void GetSvgBendDoubleAnchor(Anchor anchor)
	{
		string iconDiameter = anchor.Material.TypeId == 1 ? "Арм" : "⌀";

		if (anchor.BendLengthMillimeters <= ANCHOR_BEND_LENGTH_MAX)
			m_XOrigin += anchor.BendLengthMillimeters; // X origin
		else
			m_XOrigin += ANCHOR_BEND_LENGTH_MAX; // X origin

		GetScaledLength(anchor.ThreadLengthMillimeters, anchor.ThreadSecondLengthMillimeters); // Scaling ThreadLengthMillimeters

		int gap = 20; // Gap in out of max LengthMillimeters  of anchor
		int outPartHorSize = 45; // LengthMillimeters output part of horizontal size
		int outPartShortHorSize = 40; // Short LengthMillimeters output part of horizontal size
		int outPartRadSize = 45; // LengthMillimeters of shelf of radius size
		int offsetVert = 0; // Vertical offset of second part upper end
		if (anchor.LengthMillimeters <= ANCHOR_LENGTH_MAX)
			offsetVert = anchor.LengthMillimeters - anchor.SecondLengthMillimeters;
		else
		{
			if (anchor.LengthMillimeters != anchor.SecondLengthMillimeters)
				offsetVert = ANCHOR_LENGTH_MAX - ANCHOR_SECOND_LENGTH_MAX;
		}
		var svgDoc = new GcSvgDocument();
		svgDoc.RootSvg.Width = new SvgLength(WIDTH_PIXELS, SvgLengthUnits.Pixels);
		svgDoc.RootSvg.Height = new SvgLength(HEIGHT_PIXELS, SvgLengthUnits.Pixels);

		List<SvgElement> svgElements = new(); // Make list to fill with objects SvgRectElement

		if (anchor.ThreadLengthMillimeters > 0)
		{
			// Draw part with thread

			var rectThreadBodyAnchor = GetSvgRectElement(m_XOrigin,
				m_YOrigin,
				anchor.ThreadDiameterMillimeters,
				m_ScaledThreadLength,
				Color.Transparent,
				Color.Black,
				1.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(rectThreadBodyAnchor);

			var rectThreadStepBodyAnchor = GetSvgRectElement(m_XOrigin + anchor.ThreadStepMillimeters / 2,
				m_YOrigin,
				anchor.ThreadDiameterMillimeters - anchor.ThreadStepMillimeters,
				m_ScaledThreadLength,
				Color.Transparent,
				Color.Black,
				1f,
				SvgLengthUnits.Pixels);

			svgElements.Add(rectThreadStepBodyAnchor);

			// Draw second part with thread

			SvgRectElement rectThreadBodyAnchorSecond;
			SvgRectElement rectThreadStepBodyAnchorSecond;

			SvgLineElement threadAxialLineFirst;
			SvgLineElement threadAxialLineSecond;

			threadAxialLineFirst = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
									m_YOrigin - outPartHorSize,
									m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
									m_YOrigin + m_ScaledThreadLength,
									Color.Black,
									0.15f,
									SvgLengthUnits.Pixels);

			svgElements.Add(threadAxialLineFirst); // Make top part of axial thread's line of anchor

			if (anchor.BendLengthMillimeters <= ANCHOR_BEND_LENGTH_MAX)
			{
				rectThreadBodyAnchorSecond = GetSvgRectElement(m_XOrigin - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
				m_YOrigin + offsetVert,
				anchor.ThreadDiameterMillimeters,
				m_ScaledThreadLength,
				Color.Transparent,
				Color.Black,
				1.5f,
				SvgLengthUnits.Pixels);

				svgElements.Add(rectThreadBodyAnchorSecond);

				rectThreadStepBodyAnchorSecond = GetSvgRectElement(m_XOrigin + anchor.ThreadStepMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
				m_YOrigin + offsetVert,
				anchor.ThreadDiameterMillimeters - anchor.ThreadStepMillimeters,
				m_ScaledThreadLength,
				Color.Transparent,
				Color.Black,
				1f,
				SvgLengthUnits.Pixels);

				svgElements.Add(rectThreadStepBodyAnchorSecond);

				threadAxialLineSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
									m_YOrigin - outPartHorSize + offsetVert,
									m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
									m_YOrigin + m_ScaledThreadLength,
									Color.Black,
									0.15f,
									SvgLengthUnits.Pixels);

				svgElements.Add(threadAxialLineSecond); // Make top left part of axial thread's line of anchor
			}
			else
			{
				rectThreadBodyAnchorSecond = GetSvgRectElement(m_XOrigin - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + offsetVert,
								anchor.ThreadDiameterMillimeters,
								m_ScaledThreadLength,
								Color.Transparent,
								Color.Black,
								1.5f,
								SvgLengthUnits.Pixels);

				svgElements.Add(rectThreadBodyAnchorSecond);

				rectThreadStepBodyAnchorSecond = GetSvgRectElement(m_XOrigin + anchor.ThreadStepMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + offsetVert,
								anchor.ThreadDiameterMillimeters - anchor.ThreadStepMillimeters,
								m_ScaledThreadLength,
								Color.Transparent,
								Color.Black,
								1f,
								SvgLengthUnits.Pixels);

				svgElements.Add(rectThreadStepBodyAnchorSecond); // Make top left part of axial thread's line of anchor

				threadAxialLineSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin - outPartHorSize + offsetVert,
								m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + m_ScaledThreadLength,
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // --

				svgElements.Add(threadAxialLineSecond); // Make top left part of axial thread's line of anchor
			}

			// Draw sizes of part with thread

			// Size of thread's diametr

			var lineVertLeftSizeDiamThread = GetSvgLineElement(m_XOrigin,
				m_YOrigin,
				m_XOrigin,
				m_YOrigin - outPartHorSize,
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(lineVertLeftSizeDiamThread);

			var lineVertRightSizeDiamThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
					m_YOrigin,
					m_XOrigin + anchor.ThreadDiameterMillimeters,
					m_YOrigin - outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineVertRightSizeDiamThread);

			var lineHorSizeDiamThread = GetSvgLineElement(m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						m_XOrigin + anchor.ThreadDiameterMillimeters + 105,
						m_YOrigin - (outPartHorSize - 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorSizeDiamThread);

			var lineSerifLeftSizeDiamThread = GetSerif(m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						m_XOrigin,
						m_YOrigin - (outPartHorSize - 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifLeftSizeDiamThread);

			var lineSerifRightSizeDiamThread = GetSerif(m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin - (outPartHorSize - 5),
							m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin - (outPartHorSize - 5),
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifRightSizeDiamThread);


			svgElements.Add(GetSvgTextElement($"М{anchor.ThreadDiameterMillimeters}x{anchor.ThreadStepMillimeters}",
							m_XOrigin + anchor.ThreadDiameterMillimeters + 8,
							m_YOrigin - (outPartHorSize - 3),
							0,
							SvgLengthUnits.Pixels)); // Make text of size's value diametr of thread

			// Size of thread's LengthMillimeters

			var lineHorTopSizeLengthThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
						m_YOrigin,
						m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthThread);

			var lineHorBotSizeLengthThread = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength,
							m_XOrigin + anchor.ThreadDiameterMillimeters + (outPartHorSize + 5),
							m_YOrigin + m_ScaledThreadLength,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineHorBotSizeLengthThread);

			var lineVerSizeDiamThread = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin,
									m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

			svgElements.Add(lineVerSizeDiamThread);

			var lineSerifTopSizeDiamThread = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
								m_YOrigin,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
								m_YOrigin,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeDiamThread);

			var lineSerifBotSizeDiamThread = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize,
									m_YOrigin + m_ScaledThreadLength,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeDiamThread);

			svgElements.Add(GetSvgTextElement($"{anchor.ThreadLengthMillimeters}",
				m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + outPartHorSize - 2,
				m_YOrigin + m_ScaledThreadLength / 2 + 10,
				-90,
				SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of thread
		}

		// Size of anchors's diametr

		var lineHorSizeDiamAnchor = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2 + 55,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineHorSizeDiamAnchor);

		var lineSerifLeftSizeDiamAnchor = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineSerifLeftSizeDiamAnchor);

		var lineSerifRightSizeDiamAnchor = GetSerif(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2,
					m_YOrigin + m_ScaledThreadLength + outPartHorSize,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

		svgElements.Add(lineSerifRightSizeDiamAnchor);

		svgElements.Add(GetSvgTextElement($"{iconDiameter}{anchor.DiameterMillimeters}",
			m_XOrigin + anchor.ThreadDiameterMillimeters / 2 + anchor.DiameterMillimeters / 2 + 5,
			m_YOrigin + m_ScaledThreadLength + outPartHorSize - 2,
			0,
			SvgLengthUnits.Pixels)); // Make text of size's value diametr of anchor

		// Make objects basic part without thread and bend

		SvgRectElement rectBasicBodyAnchor;
		SvgRectElement rectBasicBodyAnchorSecond;

		// Make objects of sizes anchor's LengthMillimeters

		SvgLineElement lineHorTopSizeLengthOfAnchor;
		SvgLineElement lineHorBotSizeLengthOfAnchor;
		SvgLineElement lineVertSizeLengthOfAnchor;

		SvgLineElement lineHorTopSizeLengthOfAnchorWithoutRadius;
		SvgLineElement lineHorBotSizeLengthOfAnchorWithoutRadius;
		SvgLineElement lineVertSizeLengthOfAnchorWithoutRadius;

		// Make objects of sizes anchor's second LengthMillimeters

		SvgLineElement lineHorTopSizeLengthSecondOfAnchor;
		SvgLineElement lineHorBotSizeLengthSecondOfAnchor;
		SvgLineElement lineVertSizeLengthSecondOfAnchor;

		// Make object bending part without radius

		SvgRectElement rectBendAnchor;

		// Make objects bending part with radius

		var pbRadiusBend = new SvgPathBuilder();
		var pathRadiusBend = new SvgPathElement();
		var pbRadiusBendSecond = new SvgPathBuilder();
		var pathRadiusBendSecond = new SvgPathElement();

		SvgLineElement basicBodyTopAxialLineFirst;
		SvgLineElement basicBodyTopAxialLineSecond;
		SvgLineElement middleAxialLine;

		SvgLineElement bendPartWithoutRadiusAxialLineRight;
		SvgLineElement bendPartWithoutRadiusAxialLineLeft;

		SvgLineElement basicBodyBotAxialLineFirst;
		SvgLineElement basicBodyBotAxialLineSecond;

		SvgPathBuilder pbAxialBendRadiusRightfOfAnchor = new();
		SvgPathElement pathAxialBendRadiusRightfOfAnchor = new();

		SvgPathBuilder pbAxialBendRadiusLeftfOfAnchor = new();
		SvgPathElement pathAxialBendRadiusLeftfOfAnchor = new();

		if (anchor.LengthMillimeters <= ANCHOR_LENGTH_MAX)
		{
			basicBodyTopAxialLineFirst = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
								m_YOrigin + m_ScaledThreadLength,
								m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
								m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line in right side of anchor

			svgElements.Add(basicBodyTopAxialLineFirst);

			pbAxialBendRadiusRightfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + anchor.LengthMillimeters - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters));
			pbAxialBendRadiusRightfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + anchor.LengthMillimeters - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters),
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2);

			pathAxialBendRadiusRightfOfAnchor.PathData = pbAxialBendRadiusRightfOfAnchor.ToPathData();
			pathAxialBendRadiusRightfOfAnchor.Fill = new SvgPaint(Color.Transparent);
			pathAxialBendRadiusRightfOfAnchor.Stroke = new SvgPaint(Color.Black);
			pathAxialBendRadiusRightfOfAnchor.StrokeWidth = new SvgLength(0.15f);

			svgElements.Add(pathAxialBendRadiusRightfOfAnchor); // Drawing of axial line in right bend radius of anchor

			// Draw basic part without thread and bend

			rectBasicBodyAnchor = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + m_ScaledThreadLength,
				anchor.DiameterMillimeters,
				anchor.LengthMillimeters - (m_ScaledThreadLength + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
				Color.Transparent,
				Color.Black,
				1.5f,
				SvgLengthUnits.Pixels);

			if (anchor.BendLengthMillimeters <= ANCHOR_BEND_LENGTH_MAX)
			{
				pbAxialBendRadiusLeftfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + anchor.LengthMillimeters - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters));
				pbAxialBendRadiusLeftfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + anchor.LengthMillimeters - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters),
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters + anchor.DiameterMillimeters / 2 + anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2);

				pathAxialBendRadiusLeftfOfAnchor.PathData = pbAxialBendRadiusLeftfOfAnchor.ToPathData();
				pathAxialBendRadiusLeftfOfAnchor.Fill = new SvgPaint(Color.Transparent);
				pathAxialBendRadiusLeftfOfAnchor.Stroke = new SvgPaint(Color.Black);
				pathAxialBendRadiusLeftfOfAnchor.StrokeWidth = new SvgLength(0.15f);

				svgElements.Add(pathAxialBendRadiusLeftfOfAnchor); // Drawing of axial line in left bend radius of anchor

				basicBodyTopAxialLineSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
								m_YOrigin + m_ScaledThreadLength,
								m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
								m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line in left side of anchor

				svgElements.Add(basicBodyTopAxialLineSecond);

				// Draw second basic part without thread and bend

				rectBasicBodyAnchorSecond = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + offsetVert,
					anchor.DiameterMillimeters,
					anchor.LengthMillimeters - (m_ScaledThreadLength + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) - offsetVert,
					Color.Transparent,
					Color.Black,
					1.5f,
					SvgLengthUnits.Pixels);
					
				// Size of anchors's second LengthMillimeters

				lineHorTopSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthSecondOfAnchor);


				lineHorBotSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) - anchor.BendLengthMillimeters,
									m_YOrigin + anchor.LengthMillimeters,
									m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartHorSize - anchor.BendLengthMillimeters,
									m_YOrigin + anchor.LengthMillimeters,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

				svgElements.Add(lineHorBotSizeLengthSecondOfAnchor);

				lineVertSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineVertSizeLengthSecondOfAnchor);

				var lineSerifTopSizeLengthSecondOfAnchor = GetSerif(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeLengthSecondOfAnchor);

				var lineSerifBotSizeLengthSecondOfAnchor = GetSerif(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifBotSizeLengthSecondOfAnchor);

				svgElements.Add(GetSvgTextElement($"{anchor.SecondLengthMillimeters}",
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - 2 - anchor.BendLengthMillimeters,
							m_YOrigin + (anchor.LengthMillimeters + offsetVert) / 2 + 10,
							-90,
							SvgLengthUnits.Pixels)); // Make text of size's value second LengthMillimeters of anchor

				// Draw bending part without radius

				rectBendAnchor = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters,
					anchor.BendLengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) * 2,
					anchor.DiameterMillimeters,
					Color.Transparent,
					Color.Black,
					1.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(rectBendAnchor);

				bendPartWithoutRadiusAxialLineRight = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
								m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
								m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.BendRadiusMillimeters - (anchor.BendLengthMillimeters - 2 * anchor.DiameterMillimeters),
								m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line of bend part without radius

				svgElements.Add(bendPartWithoutRadiusAxialLineRight);

				middleAxialLine = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters - anchor.BendLengthMillimeters / 2,
								m_YOrigin - outPartHorSize,
								m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters - anchor.BendLengthMillimeters / 2,
								m_YOrigin + anchor.LengthMillimeters + 5,
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line in middle

				svgElements.Add(middleAxialLine);

				// Size of bending part

				var lineVertLeftSizeBendPart = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) + outPartHorSize,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPart);

				var lineVertRightSizeBendPart = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) + outPartHorSize,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPart);

				var lineHorSizeBendPart = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPart);

				var lineSerifLeftSizeBendPart = GetSerif(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPart);

				var lineSerifRightSizeBendPart = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPart);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - anchor.BendLengthMillimeters) / 2 - 10,
						m_YOrigin + anchor.LengthMillimeters + outPartHorSize - 2 + outPartHorSize,
						0,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of thread

				// Size of bending part without radius

				var lineVertLeftSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPartWithoutRadius);

				var lineVertRightSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPartWithoutRadius);

				var lineHorSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPartWithoutRadius);

				var lineSerifLeftSizeBendPartWithoutRadius = GetSerif(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPartWithoutRadius);

				var lineSerifRightSizeBendPartWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPartWithoutRadius);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters - 2 * (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters)}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - anchor.BendLengthMillimeters) / 2 - 10,
						m_YOrigin + anchor.LengthMillimeters + outPartHorSize - 2,
						0,
						SvgLengthUnits.Pixels)); // Make text of size of bending part without radius

				// Draw second bending part with radius

				pbRadiusBendSecond.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
				pbRadiusBendSecond.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters);
				pbRadiusBendSecond.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters - anchor.DiameterMillimeters);
				pbRadiusBendSecond.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters - anchor.DiameterMillimeters,
					m_YOrigin + anchor.LengthMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters);


				pathRadiusBendSecond.PathData = pbRadiusBendSecond.ToPathData();
				pathRadiusBendSecond.Fill = new SvgPaint(Color.Transparent);
				pathRadiusBendSecond.Stroke = new SvgPaint(Color.Black);
				pathRadiusBendSecond.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathRadiusBendSecond);
			}
			else
			{
				pbAxialBendRadiusLeftfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + anchor.LengthMillimeters - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters));
				pbAxialBendRadiusLeftfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + anchor.LengthMillimeters - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters),
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters / 2 + anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2);

				pathAxialBendRadiusLeftfOfAnchor.PathData = pbAxialBendRadiusLeftfOfAnchor.ToPathData();
				pathAxialBendRadiusLeftfOfAnchor.Fill = new SvgPaint(Color.Transparent);
				pathAxialBendRadiusLeftfOfAnchor.Stroke = new SvgPaint(Color.Black);
				pathAxialBendRadiusLeftfOfAnchor.StrokeWidth = new SvgLength(0.15f);

				svgElements.Add(pathAxialBendRadiusLeftfOfAnchor); // Drawing of axial line in left bend radius of anchor

				basicBodyTopAxialLineSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + m_ScaledThreadLength,
								m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line in left side of anchor

				svgElements.Add(basicBodyTopAxialLineSecond);

				middleAxialLine = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
								m_YOrigin - outPartHorSize,
								m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
								m_YOrigin + anchor.LengthMillimeters + 5,
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line in middle

				svgElements.Add(middleAxialLine);

				// Draw second basic part without thread and bend

				rectBasicBodyAnchorSecond = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + offsetVert,
					anchor.DiameterMillimeters,
					anchor.LengthMillimeters - (m_ScaledThreadLength + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) - offsetVert,
					Color.Transparent,
					Color.Black,
					1.5f,
					SvgLengthUnits.Pixels);

				// Size of anchors's second LengthMillimeters

				lineHorTopSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthSecondOfAnchor);


				lineHorBotSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) - ANCHOR_BEND_LENGTH_MAX,
									m_YOrigin + anchor.LengthMillimeters,
									m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartHorSize - ANCHOR_BEND_LENGTH_MAX,
									m_YOrigin + anchor.LengthMillimeters,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

				svgElements.Add(lineHorBotSizeLengthSecondOfAnchor);

				lineVertSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineVertSizeLengthSecondOfAnchor);

				var lineSerifTopSizeLengthSecondOfAnchor = GetSerif(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeLengthSecondOfAnchor);

				var lineSerifBotSizeLengthSecondOfAnchor = GetSerif(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifBotSizeLengthSecondOfAnchor);

				svgElements.Add(GetSvgTextElement($"{anchor.SecondLengthMillimeters}",
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + (anchor.LengthMillimeters + offsetVert) / 2 + 10,
							-90,
							SvgLengthUnits.Pixels)); // Make text of size's value second LengthMillimeters of anchor

				// Draw bending part without radius

				// Make right half bending part without radius

				var pbHalfRightBendPartAnchor = new SvgPathBuilder();
				var pathHalfRightBendPartAnchor = new SvgPathElement();

				pbHalfRightBendPartAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
				pbHalfRightBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters);
				pbHalfRightBendPartAnchor.AddVerticalLineTo(false, m_YOrigin + anchor.LengthMillimeters);
				pbHalfRightBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap);

				pathHalfRightBendPartAnchor.PathData = pbHalfRightBendPartAnchor.ToPathData();
				pathHalfRightBendPartAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfRightBendPartAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfRightBendPartAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfRightBendPartAnchor);

				// Make left half bending part without radius

				var pbHalfLeftBendPartAnchor = new SvgPathBuilder();
				var pathHalfLeftBendPartAnchor = new SvgPathElement();

				pbHalfLeftBendPartAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
				pbHalfLeftBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters);
				pbHalfLeftBendPartAnchor.AddVerticalLineTo(false, m_YOrigin + anchor.LengthMillimeters);
				pbHalfLeftBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2);

				pathHalfLeftBendPartAnchor.PathData = pbHalfLeftBendPartAnchor.ToPathData();
				pathHalfLeftBendPartAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfLeftBendPartAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfLeftBendPartAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfLeftBendPartAnchor);

				// Make gap Right Line

				var pbgapRight = new SvgPathBuilder();
				var pathgapRight = new SvgPathElement();

				pbgapRight.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
				pbgapRight.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap - 5,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
					m_YOrigin + anchor.LengthMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
					m_YOrigin + anchor.LengthMillimeters);

				pathgapRight.PathData = pbgapRight.ToPathData();
				pathgapRight.Fill = new SvgPaint(Color.Transparent);
				pathgapRight.Stroke = new SvgPaint(Color.Black);
				pathgapRight.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapRight);

				bendPartWithoutRadiusAxialLineRight = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
						Color.Black,
						0.15f,
						SvgLengthUnits.Pixels); // Drawing of axial line of bend right part without radius

				svgElements.Add(bendPartWithoutRadiusAxialLineRight);

				bendPartWithoutRadiusAxialLineLeft = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
					Color.Black,
					0.15f,
					SvgLengthUnits.Pixels); // Drawing of axial line of bend left part without radius

				svgElements.Add(bendPartWithoutRadiusAxialLineLeft);

				// Make gap Left Line

				var pbgapLeft = new SvgPathBuilder();
				var pathgapLeft = new SvgPathElement();

				pbgapLeft.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
				pbgapLeft.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 - 5,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters / 2,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + anchor.LengthMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + anchor.LengthMillimeters);

				pathgapLeft.PathData = pbgapLeft.ToPathData();
				pathgapLeft.Fill = new SvgPaint(Color.Transparent);
				pathgapLeft.Stroke = new SvgPaint(Color.Black);
				pathgapLeft.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapLeft);

				// Size of bending part

				var lineVertLeftSizeBendPart = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) + outPartHorSize,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPart);

				var lineVertRightSizeBendPart = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5) + outPartHorSize,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPart);

				var lineHorSizeBendPart = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPart);

				var lineSerifLeftSizeBendPart = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPart);

				var lineSerifRightSizeBendPart = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPart);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - ANCHOR_BEND_LENGTH_MAX) / 2 - 10,
						m_YOrigin + anchor.LengthMillimeters + outPartHorSize - 2 + outPartHorSize,
						0,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of thread

				// Size of bending part without radius

				var lineVertLeftSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPartWithoutRadius);

				var lineVertRightSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
						m_YOrigin + anchor.LengthMillimeters + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPartWithoutRadius);

				var lineHorSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPartWithoutRadius);

				var lineSerifLeftSizeBendPartWithoutRadius = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPartWithoutRadius);

				var lineSerifRightSizeBendPartWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
							m_YOrigin + anchor.LengthMillimeters + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPartWithoutRadius);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters - 2 * (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters)}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - ANCHOR_BEND_LENGTH_MAX) / 2 - 10,
						m_YOrigin + anchor.LengthMillimeters + outPartHorSize - 2,
						0,
						SvgLengthUnits.Pixels)); // Make text of size of bending part without radius

				// Draw second bending part with radius

				pbRadiusBendSecond.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
				pbRadiusBendSecond.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters);
				pbRadiusBendSecond.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + - ANCHOR_BEND_LENGTH_MAX);
				pbRadiusBendSecond.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + anchor.LengthMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters);


				pathRadiusBendSecond.PathData = pbRadiusBendSecond.ToPathData();
				pathRadiusBendSecond.Fill = new SvgPaint(Color.Transparent);
				pathRadiusBendSecond.Stroke = new SvgPaint(Color.Black);
				pathRadiusBendSecond.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathRadiusBendSecond);
			}

			svgElements.Add(rectBasicBodyAnchorSecond);

			// Draw bending part with radius

			pbRadiusBend.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters));
			pbRadiusBend.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters,
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
				m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters,
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
				m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters);
			pbRadiusBend.AddVerticalLineTo(false, m_YOrigin + anchor.LengthMillimeters);
			pbRadiusBend.AddCurveTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
				m_YOrigin + anchor.LengthMillimeters,
				m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
				m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
				m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters));


			pathRadiusBend.PathData = pbRadiusBend.ToPathData();
			pathRadiusBend.Fill = new SvgPaint(Color.Transparent);
			pathRadiusBend.Stroke = new SvgPaint(Color.Black);
			pathRadiusBend.StrokeWidth = new SvgLength(1.5f);

			svgElements.Add(pathRadiusBend);

			// Size of radius

			var lineInclinSizeRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineInclinSizeRadius);

			var lineHorSizeRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
				m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - outPartRadSize,
				m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(lineHorSizeRadius);

			var lineSerifSizeRadius = GetSerifRad(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifSizeRadius);

			svgElements.Add(GetSvgTextElement($"R{anchor.BendRadiusMillimeters}",
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - outPartRadSize,
				m_YOrigin + anchor.LengthMillimeters - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters - 2,
				0,
				SvgLengthUnits.Pixels)); // Make text of size's value radius of anchor

			svgElements.Add(rectBasicBodyAnchor);

			// Size of anchors's LengthMillimeters

			lineHorTopSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize) + outPartHorSize,
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthOfAnchor);


			lineHorBotSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								m_YOrigin + anchor.LengthMillimeters,
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize) + outPartHorSize,
								m_YOrigin + anchor.LengthMillimeters,
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

			svgElements.Add(lineHorBotSizeLengthOfAnchor);

			lineVertSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
						m_YOrigin + anchor.LengthMillimeters,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineVertSizeLengthOfAnchor);

			var lineSerifTopSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
						m_YOrigin,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeLengthOfAnchor);

			var lineSerifBotSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
						m_YOrigin + anchor.LengthMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
						m_YOrigin + anchor.LengthMillimeters,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeLengthOfAnchor);

			svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters}",
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2 + outPartHorSize,
						m_YOrigin + anchor.LengthMillimeters / 2 + 10,
						-90,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor
				
			// Size of anchors's LengthMillimeters without radius

			lineHorTopSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthOfAnchorWithoutRadius);


			lineHorBotSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
								m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
								m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								Color.Black,
								0.5f,
								SvgLengthUnits.Pixels);

			svgElements.Add(lineHorBotSizeLengthOfAnchorWithoutRadius);

			lineVertSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineVertSizeLengthOfAnchorWithoutRadius);

			var lineSerifTopSizeLengthOfAnchorWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeLengthOfAnchorWithoutRadius);

			var lineSerifBotSizeLengthOfAnchorWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin + anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeLengthOfAnchorWithoutRadius);

			svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters)}",
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2,
						m_YOrigin + (anchor.LengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters)) / 2 + 10,
						-90,
						SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor without radius
		}
		else
		{
			pbAxialBendRadiusRightfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters));
			pbAxialBendRadiusRightfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters),
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2);

			pathAxialBendRadiusRightfOfAnchor.PathData = pbAxialBendRadiusRightfOfAnchor.ToPathData();
			pathAxialBendRadiusRightfOfAnchor.Fill = new SvgPaint(Color.Transparent);
			pathAxialBendRadiusRightfOfAnchor.Stroke = new SvgPaint(Color.Black);
			pathAxialBendRadiusRightfOfAnchor.StrokeWidth = new SvgLength(0.15f);

			svgElements.Add(pathAxialBendRadiusRightfOfAnchor); // Drawing of axial line in right bend radius of anchor

			basicBodyTopAxialLineFirst = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
								m_YOrigin + m_ScaledThreadLength,
								m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
								m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line in right side of anchor

			svgElements.Add(basicBodyTopAxialLineFirst);

			basicBodyBotAxialLineFirst = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
							m_XOrigin + anchor.ThreadDiameterMillimeters / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							Color.Black,
							0.15f,
							SvgLengthUnits.Pixels); // Drawing of axial line in right bottom side of anchor

			svgElements.Add(basicBodyBotAxialLineFirst);

			// Draw basic part without thread and bend

			// Make top half basic part without thread and bend

			var pbHalfTopBasicBodyAnchor = new SvgPathBuilder();
			var pathHalfTopBasicBodyAnchor = new SvgPathElement();

			pbHalfTopBasicBodyAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
			pbHalfTopBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength);
			pbHalfTopBasicBodyAnchor.AddHorizontalLineTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2);
			pbHalfTopBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

			pathHalfTopBasicBodyAnchor.PathData = pbHalfTopBasicBodyAnchor.ToPathData();
			pathHalfTopBasicBodyAnchor.Fill = new SvgPaint(Color.Transparent);
			pathHalfTopBasicBodyAnchor.Stroke = new SvgPaint(Color.Black);
			pathHalfTopBasicBodyAnchor.StrokeWidth = new SvgLength(1.5f);

			svgElements.Add(pathHalfTopBasicBodyAnchor);

			// Make gap Top Line

			var pbgapTop = new SvgPathBuilder();
			var pathgapTop = new SvgPathElement();
			float halfDiam;

			if (anchor.ThreadLengthMillimeters > 0)
				halfDiam = anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters / 2;
			else
				halfDiam = 0;

			pbgapTop.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
			pbgapTop.AddCurveTo(false, m_XOrigin + halfDiam,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap) - 5,
				m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
				m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
				m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

			pathgapTop.PathData = pbgapTop.ToPathData();
			pathgapTop.Fill = new SvgPaint(Color.Transparent);
			pathgapTop.Stroke = new SvgPaint(Color.Black);
			pathgapTop.StrokeWidth = new SvgLength(0.5f);

			svgElements.Add(pathgapTop);

			SvgLineElement lineSerifBotSizeLengthOfAnchor;

			// Make gap Bot Line

			var pbgapBot = new SvgPathBuilder();
			var pathgapBot = new SvgPathElement();

			pbgapBot.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
			pbgapBot.AddCurveTo(false, m_XOrigin + halfDiam,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2 - 5,
				m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
				m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

			pathgapBot.PathData = pbgapBot.ToPathData();
			pathgapBot.Fill = new SvgPaint(Color.Transparent);
			pathgapBot.Stroke = new SvgPaint(Color.Black);
			pathgapBot.StrokeWidth = new SvgLength(0.5f);

			svgElements.Add(pathgapBot);

			var pbHalfBotBasicBodyAnchor = new SvgPathBuilder();
			var pathHalfBotBasicBodyAnchor = new SvgPathElement();

			// Make bottom half basic part without thread and bend

			pbHalfBotBasicBodyAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
			pbHalfBotBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters);
			pbHalfBotBasicBodyAnchor.AddHorizontalLineTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2);
			pbHalfBotBasicBodyAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

			pathHalfBotBasicBodyAnchor.PathData = pbHalfBotBasicBodyAnchor.ToPathData();
			pathHalfBotBasicBodyAnchor.Fill = new SvgPaint(Color.Transparent);
			pathHalfBotBasicBodyAnchor.Stroke = new SvgPaint(Color.Black);
			pathHalfBotBasicBodyAnchor.StrokeWidth = new SvgLength(1.5f);

			svgElements.Add(pathHalfBotBasicBodyAnchor);

			// Draw bending part with radius

			pbRadiusBend.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters);
			pbRadiusBend.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters,
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters,
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
			pbRadiusBend.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX);
			pbRadiusBend.AddCurveTo(false, m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
				m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
				m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters) / 2,
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters);

			pathRadiusBend.PathData = pbRadiusBend.ToPathData();
			pathRadiusBend.Fill = new SvgPaint(Color.Transparent);
			pathRadiusBend.Stroke = new SvgPaint(Color.Black);
			pathRadiusBend.StrokeWidth = new SvgLength(1.5f);

			svgElements.Add(pathRadiusBend);

			// Make obj top half second basic part without thread and bend

			var pbHalfTopBasicBodySecondAnchor = new SvgPathBuilder();
			var pathHalfTopBasicBodySecondAnchor = new SvgPathElement();

			// Make obj gap Top Line of second basic part without thread and bend

			var pbgapTopSecond = new SvgPathBuilder();
			var pathgapTopSecond = new SvgPathElement();

			// Make obj gap Bot Line of second basic part without thread and bend

			var pbgapBotSecond = new SvgPathBuilder();
			var pathgapBotSecond = new SvgPathElement();

			// Make obj bot half second basic part without thread and bend

			var pbHalfBotBasicBodySecondAnchor = new SvgPathBuilder();
			var pathHalfBotBasicBodySecondAnchor = new SvgPathElement();

			if (anchor.BendLengthMillimeters <= ANCHOR_BEND_LENGTH_MAX)
			{
				pbAxialBendRadiusLeftfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters));
				pbAxialBendRadiusLeftfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters),
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters / 2 + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2);

				pathAxialBendRadiusLeftfOfAnchor.PathData = pbAxialBendRadiusLeftfOfAnchor.ToPathData();
				pathAxialBendRadiusLeftfOfAnchor.Fill = new SvgPaint(Color.Transparent);
				pathAxialBendRadiusLeftfOfAnchor.Stroke = new SvgPaint(Color.Black);
				pathAxialBendRadiusLeftfOfAnchor.StrokeWidth = new SvgLength(0.15f);

				svgElements.Add(pathAxialBendRadiusLeftfOfAnchor); // Drawing of axial line in left bend radius of anchor

				basicBodyTopAxialLineSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + m_ScaledThreadLength,
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
						Color.Black,
						0.15f,
						SvgLengthUnits.Pixels); // Drawing of axial line in left side of anchor

				svgElements.Add(basicBodyTopAxialLineSecond);

				bendPartWithoutRadiusAxialLineRight = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.BendRadiusMillimeters - (anchor.BendLengthMillimeters - 2 * anchor.DiameterMillimeters),
				m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
				Color.Black,
				0.15f,
				SvgLengthUnits.Pixels); // Drawing of axial line of bend part without radius

				svgElements.Add(bendPartWithoutRadiusAxialLineRight);

				basicBodyBotAxialLineSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
								m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line in left bottom side of anchor

				svgElements.Add(basicBodyBotAxialLineSecond);

				// Draw second basic part without thread and bend

				// Make bottom half basic part without thread and bend

				pbHalfBotBasicBodySecondAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbHalfBotBasicBodySecondAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters);
				pbHalfBotBasicBodySecondAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters + 2 * anchor.DiameterMillimeters);
				pbHalfBotBasicBodySecondAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathHalfBotBasicBodySecondAnchor.PathData = pbHalfBotBasicBodySecondAnchor.ToPathData();
				pathHalfBotBasicBodySecondAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfBotBasicBodySecondAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfBotBasicBodySecondAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfBotBasicBodySecondAnchor);

				// Make gap Bot Line

				pbgapBotSecond.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbgapBotSecond.AddCurveTo(false, m_XOrigin + halfDiam - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2 - 5,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathgapBotSecond.PathData = pbgapBotSecond.ToPathData();
				pathgapBotSecond.Fill = new SvgPaint(Color.Transparent);
				pathgapBotSecond.Stroke = new SvgPaint(Color.Black);
				pathgapBotSecond.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapBotSecond);

				// Make top half basic part without thread and bend

				pbHalfTopBasicBodySecondAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
				pbHalfTopBasicBodySecondAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + offsetVert);
				pbHalfTopBasicBodySecondAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters + 2 * anchor.DiameterMillimeters);
				pbHalfTopBasicBodySecondAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

				pathHalfTopBasicBodySecondAnchor.PathData = pbHalfTopBasicBodySecondAnchor.ToPathData();
				pathHalfTopBasicBodySecondAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfTopBasicBodySecondAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfTopBasicBodySecondAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfTopBasicBodySecondAnchor);

				// Make gap Top Line

				pbgapTopSecond.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
						m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
				pbgapTopSecond.AddCurveTo(false, m_XOrigin + halfDiam - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap) - 5,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

				pathgapTopSecond.PathData = pbgapTopSecond.ToPathData();
				pathgapTopSecond.Fill = new SvgPaint(Color.Transparent);
				pathgapTopSecond.Stroke = new SvgPaint(Color.Black);
				pathgapTopSecond.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapTopSecond);

				middleAxialLine = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters / 2 + anchor.DiameterMillimeters,
							m_YOrigin - outPartHorSize,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendLengthMillimeters / 2 + anchor.DiameterMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + 5,
							Color.Black,
							0.15f,
							SvgLengthUnits.Pixels); // Drawing of axial line in middle

				svgElements.Add(middleAxialLine);

				// Size of anchors's second LengthMillimeters

				lineHorTopSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthSecondOfAnchor);


				lineHorBotSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) - anchor.BendLengthMillimeters,
									m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
									m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartHorSize - anchor.BendLengthMillimeters,
									m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

				svgElements.Add(lineHorBotSizeLengthSecondOfAnchor);

				lineVertSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineVertSizeLengthSecondOfAnchor);

				var lineSerifTopSizeLengthSecondOfAnchor = GetSerif(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + offsetVert,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeLengthSecondOfAnchor);

				var lineSerifBotSizeLengthSecondOfAnchor = GetSerif(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifBotSizeLengthSecondOfAnchor);

				svgElements.Add(GetSvgTextElement($"{anchor.SecondLengthMillimeters}",
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - 2 - anchor.BendLengthMillimeters,
							m_YOrigin + (m_ScaledThreadLength + ANCHOR_LENGTH_MAX + offsetVert) / 2 + 10,
							-90,
							SvgLengthUnits.Pixels)); // Make text of size's value second LengthMillimeters of anchor

				// Draw bending part without radius

				rectBendAnchor = GetSvgRectElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters,
					anchor.BendLengthMillimeters - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) * 2,
					anchor.DiameterMillimeters,
					Color.Transparent,
					Color.Black,
					1.5f,
					SvgLengthUnits.Pixels);

				svgElements.Add(rectBendAnchor);

				// Size of bending part

				var lineVertLeftSizeBendPart = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + (outPartHorSize + 5) + outPartHorSize,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPart);

				var lineVertRightSizeBendPart = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + (outPartHorSize + 5) + outPartHorSize,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPart);

				var lineHorSizeBendPart = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPart);

				var lineSerifLeftSizeBendPart = GetSerif(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
							m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPart);

				var lineSerifRightSizeBendPart = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPart);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - anchor.BendLengthMillimeters) / 2 - 10,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize - 2 + outPartHorSize,
						0,
						SvgLengthUnits.Pixels)); // Make text of size of bending part

				// Size of bending part without radius

				var lineVertLeftSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPartWithoutRadius);

				var lineVertRightSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPartWithoutRadius);

				var lineHorSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + anchor.DiameterMillimeters + anchor.BendRadiusMillimeters,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPartWithoutRadius);

				var lineSerifLeftSizeBendPartWithoutRadius = GetSerif(m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - anchor.BendLengthMillimeters + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPartWithoutRadius);

				var lineSerifRightSizeBendPartWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPartWithoutRadius);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters - 2 * (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters)}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - anchor.BendLengthMillimeters) / 2 - 10,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + outPartHorSize - 2,
						0,
						SvgLengthUnits.Pixels)); // Make text of size of bending part without radius

				// Draw second bending part with radius

				pbRadiusBendSecond.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters);
				pbRadiusBendSecond.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters);
				pbRadiusBendSecond.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters - anchor.DiameterMillimeters);
				pbRadiusBendSecond.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters - anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + 2 * anchor.DiameterMillimeters - anchor.BendLengthMillimeters + anchor.BendRadiusMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX);


				pathRadiusBendSecond.PathData = pbRadiusBendSecond.ToPathData();
				pathRadiusBendSecond.Fill = new SvgPaint(Color.Transparent);
				pathRadiusBendSecond.Stroke = new SvgPaint(Color.Black);
				pathRadiusBendSecond.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathRadiusBendSecond);
			}
			else
			{
				pbAxialBendRadiusLeftfOfAnchor.AddMoveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters));
				pbAxialBendRadiusLeftfOfAnchor.AddCurveTo(false, m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters),
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
						m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters / 2 + anchor.BendRadiusMillimeters,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2);

				pathAxialBendRadiusLeftfOfAnchor.PathData = pbAxialBendRadiusLeftfOfAnchor.ToPathData();
				pathAxialBendRadiusLeftfOfAnchor.Fill = new SvgPaint(Color.Transparent);
				pathAxialBendRadiusLeftfOfAnchor.Stroke = new SvgPaint(Color.Black);
				pathAxialBendRadiusLeftfOfAnchor.StrokeWidth = new SvgLength(0.15f);

				svgElements.Add(pathAxialBendRadiusLeftfOfAnchor); // Drawing of axial line in left bend radius of anchor

				basicBodyTopAxialLineSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength,
					m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
					Color.Black,
					0.15f,
					SvgLengthUnits.Pixels); // Drawing of axial line in left side of anchor

				svgElements.Add(basicBodyTopAxialLineSecond);

				basicBodyBotAxialLineSecond = GetSvgLineElement(m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
								m_XOrigin + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
								Color.Black,
								0.15f,
								SvgLengthUnits.Pixels); // Drawing of axial line in left bottom side of anchor

				svgElements.Add(basicBodyBotAxialLineSecond);

				bendPartWithoutRadiusAxialLineRight = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
						Color.Black,
						0.15f,
						SvgLengthUnits.Pixels); // Drawing of axial line of bend right part without radius

				svgElements.Add(bendPartWithoutRadiusAxialLineRight);

				bendPartWithoutRadiusAxialLineLeft = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
					Color.Black,
					0.15f,
					SvgLengthUnits.Pixels); // Drawing of axial line of bend left part without radius

				svgElements.Add(bendPartWithoutRadiusAxialLineLeft);


				// Draw second basic part without thread and bend

				// Make bottom half basic part without thread and bend

				pbHalfBotBasicBodySecondAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
								m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbHalfBotBasicBodySecondAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters);
				pbHalfBotBasicBodySecondAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters);
				pbHalfBotBasicBodySecondAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathHalfBotBasicBodySecondAnchor.PathData = pbHalfBotBasicBodySecondAnchor.ToPathData();
				pathHalfBotBasicBodySecondAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfBotBasicBodySecondAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfBotBasicBodySecondAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfBotBasicBodySecondAnchor);

				// Make gap Bot Line

				pbgapBotSecond.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);
				pbgapBotSecond.AddCurveTo(false, m_XOrigin + halfDiam - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2 - 5,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX / 2);

				pathgapBotSecond.PathData = pbgapBotSecond.ToPathData();
				pathgapBotSecond.Fill = new SvgPaint(Color.Transparent);
				pathgapBotSecond.Stroke = new SvgPaint(Color.Black);
				pathgapBotSecond.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapBotSecond);

				// Make top half basic part without thread and bend

				pbHalfTopBasicBodySecondAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
				pbHalfTopBasicBodySecondAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + offsetVert);
				pbHalfTopBasicBodySecondAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters);
				pbHalfTopBasicBodySecondAnchor.AddVerticalLineTo(false, m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

				pathHalfTopBasicBodySecondAnchor.PathData = pbHalfTopBasicBodySecondAnchor.ToPathData();
				pathHalfTopBasicBodySecondAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfTopBasicBodySecondAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfTopBasicBodySecondAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfTopBasicBodySecondAnchor);

				// Make gap Top Line

				pbgapTopSecond.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));
				pbgapTopSecond.AddCurveTo(false, m_XOrigin + halfDiam - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap) - 5,
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap),
					m_XOrigin + anchor.DiameterMillimeters / 2 + anchor.ThreadDiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + (ANCHOR_LENGTH_MAX / 2 - gap));

				pathgapTopSecond.PathData = pbgapTopSecond.ToPathData();
				pathgapTopSecond.Fill = new SvgPaint(Color.Transparent);
				pathgapTopSecond.Stroke = new SvgPaint(Color.Black);
				pathgapTopSecond.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapTopSecond);

				// Draw bending part without radius

				// Make right half bending part without radius

				var pbHalfRightBendPartAnchor = new SvgPathBuilder();
				var pathHalfRightBendPartAnchor = new SvgPathElement();

				pbHalfRightBendPartAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
				pbHalfRightBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters);
				pbHalfRightBendPartAnchor.AddVerticalLineTo(false, m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength);
				pbHalfRightBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap);

				pathHalfRightBendPartAnchor.PathData = pbHalfRightBendPartAnchor.ToPathData();
				pathHalfRightBendPartAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfRightBendPartAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfRightBendPartAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfRightBendPartAnchor);

				// Make left half bending part without radius

				var pbHalfLeftBendPartAnchor = new SvgPathBuilder();
				var pathHalfLeftBendPartAnchor = new SvgPathElement();

				pbHalfLeftBendPartAnchor.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
				pbHalfLeftBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters);
				pbHalfLeftBendPartAnchor.AddVerticalLineTo(false, m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength);
				pbHalfLeftBendPartAnchor.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2);

				pathHalfLeftBendPartAnchor.PathData = pbHalfLeftBendPartAnchor.ToPathData();
				pathHalfLeftBendPartAnchor.Fill = new SvgPaint(Color.Transparent);
				pathHalfLeftBendPartAnchor.Stroke = new SvgPaint(Color.Black);
				pathHalfLeftBendPartAnchor.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathHalfLeftBendPartAnchor);

				// Make gap Right Line

				var pbgapRight = new SvgPathBuilder();
				var pathgapRight = new SvgPathElement();

				pbgapRight.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
				pbgapRight.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap - 5,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 + gap,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength);

				pathgapRight.PathData = pbgapRight.ToPathData();
				pathgapRight.Fill = new SvgPaint(Color.Transparent);
				pathgapRight.Stroke = new SvgPaint(Color.Black);
				pathgapRight.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapRight);

				// Make gap Left Line

				var pbgapLeft = new SvgPathBuilder();
				var pathgapLeft = new SvgPathElement();

				pbgapLeft.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters);
				pbgapLeft.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2 - 5,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.DiameterMillimeters / 2,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength);

				pathgapLeft.PathData = pbgapLeft.ToPathData();
				pathgapLeft.Fill = new SvgPaint(Color.Transparent);
				pathgapLeft.Stroke = new SvgPaint(Color.Black);
				pathgapLeft.StrokeWidth = new SvgLength(0.5f);

				svgElements.Add(pathgapLeft);

				// Size of anchors's second LengthMillimeters

				lineHorTopSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorTopSizeLengthSecondOfAnchor);


				lineHorBotSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) - ANCHOR_BEND_LENGTH_MAX,
									m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
									m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartHorSize - ANCHOR_BEND_LENGTH_MAX,
									m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
									Color.Black,
									0.5f,
									SvgLengthUnits.Pixels);

				svgElements.Add(lineHorBotSizeLengthSecondOfAnchor);

				lineVertSizeLengthSecondOfAnchor = GetSvgLineElement(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineVertSizeLengthSecondOfAnchor);

				var lineSerifTopSizeLengthSecondOfAnchor = GetSerif(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + offsetVert,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifTopSizeLengthSecondOfAnchor);

				var lineSerifBotSizeLengthSecondOfAnchor = GetSerif(m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifBotSizeLengthSecondOfAnchor);

				svgElements.Add(GetSvgTextElement($"{anchor.SecondLengthMillimeters}",
							m_XOrigin - (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - outPartShortHorSize - 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + (m_ScaledThreadLength + ANCHOR_LENGTH_MAX + offsetVert) / 2 + 10,
							-90,
							SvgLengthUnits.Pixels)); // Make text of size's value second LengthMillimeters of anchor

				// Size of bending part

				var lineVertLeftSizeBendPart = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + (outPartHorSize + 5) + outPartHorSize,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPart);

				var lineVertRightSizeBendPart = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + (outPartHorSize + 5) + outPartHorSize,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPart);

				var lineHorSizeBendPart = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPart);

				var lineSerifLeftSizeBendPart = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPart);

				var lineSerifRightSizeBendPart = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPart);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - ANCHOR_BEND_LENGTH_MAX) / 2 - 10,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize - 2 + outPartHorSize,
						0,
						SvgLengthUnits.Pixels)); // Make text of size of bending part

				// Size of bending part without radius

				var lineVertLeftSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
						m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertLeftSizeBendPartWithoutRadius);

				var lineVertRightSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + (outPartHorSize + 5),
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

				svgElements.Add(lineVertRightSizeBendPartWithoutRadius);

				var lineHorSizeBendPartWithoutRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineHorSizeBendPartWithoutRadius);

				var lineSerifLeftSizeBendPartWithoutRadius = GetSerif(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifLeftSizeBendPartWithoutRadius);

				var lineSerifRightSizeBendPartWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

				svgElements.Add(lineSerifRightSizeBendPartWithoutRadius);

				svgElements.Add(GetSvgTextElement($"{anchor.BendLengthMillimeters - 2 * (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters)}",
						m_XOrigin + (anchor.DiameterMillimeters + anchor.ThreadDiameterMillimeters - ANCHOR_BEND_LENGTH_MAX) / 2 - 10,
						m_YOrigin + ANCHOR_LENGTH_MAX + m_ScaledThreadLength + outPartHorSize - 2,
						0,
						SvgLengthUnits.Pixels)); // Make text of size of bending part without radius


				middleAxialLine = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin - outPartHorSize,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 + anchor.DiameterMillimeters / 2 - ANCHOR_BEND_LENGTH_MAX / 2,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX + 5,
					Color.Black,
					0.15f,
					SvgLengthUnits.Pixels); // Drawing of axial line in middle

				svgElements.Add(middleAxialLine);

				// Draw second bending part with radius

				pbRadiusBendSecond.AddMoveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters);
				pbRadiusBendSecond.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters);
				pbRadiusBendSecond.AddHorizontalLineTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX);
				pbRadiusBendSecond.AddCurveTo(false, m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.DiameterMillimeters - anchor.BendRadiusMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - ANCHOR_BEND_LENGTH_MAX + anchor.BendRadiusMillimeters + anchor.DiameterMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX);


				pathRadiusBendSecond.PathData = pbRadiusBendSecond.ToPathData();
				pathRadiusBendSecond.Fill = new SvgPaint(Color.Transparent);
				pathRadiusBendSecond.Stroke = new SvgPaint(Color.Black);
				pathRadiusBendSecond.StrokeWidth = new SvgLength(1.5f);

				svgElements.Add(pathRadiusBendSecond);
			}

			// Size of radius

			var lineInclinSizeRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2) - anchor.DiameterMillimeters,
					m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineInclinSizeRadius);

			var lineHorSizeRadius = GetSvgLineElement(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - outPartRadSize,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(lineHorSizeRadius);

			var lineSerifSizeRadius = GetSerifRad(m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2) - anchor.DiameterMillimeters,
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2),
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - anchor.BendRadiusMillimeters * (1 - (float)Math.Sqrt(2) / 2) - anchor.DiameterMillimeters,
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifSizeRadius);

			svgElements.Add(GetSvgTextElement($"R{anchor.BendRadiusMillimeters}",
				m_XOrigin - (anchor.DiameterMillimeters - anchor.ThreadDiameterMillimeters) / 2 - anchor.BendRadiusMillimeters - outPartRadSize,
				m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters) - 2,
				0,
				SvgLengthUnits.Pixels)); // Make text of size's value radius of anchor

			// Size of anchors's LengthMillimeters

			lineHorTopSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize) + outPartHorSize,
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			lineHorBotSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize) + outPartHorSize,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			var lineSerifTopSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			lineSerifBotSizeLengthOfAnchor = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			lineVertSizeLengthOfAnchor = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
					m_YOrigin,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) + outPartHorSize,
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX,
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters}",
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2 + outPartHorSize,
					m_YOrigin + (m_ScaledThreadLength + ANCHOR_LENGTH_MAX) / 2 + 10,
					-90,
					SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor

			// Size of anchors's LengthMillimeters without radius

			lineHorTopSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineHorTopSizeLengthOfAnchorWithoutRadius);

			lineHorBotSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2,
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + outPartRadSize),
							m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
							Color.Black,
							0.5f,
							SvgLengthUnits.Pixels);

			svgElements.Add(lineHorBotSizeLengthOfAnchorWithoutRadius);

			var lineSerifTopSizeLengthOfAnchorWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin,
						m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
						m_YOrigin,
						Color.Black,
						0.5f,
						SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifTopSizeLengthOfAnchorWithoutRadius);

			var lineSerifBotSizeLengthOfAnchorWithoutRadius = GetSerif(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
				Color.Black,
				0.5f,
				SvgLengthUnits.Pixels);

			svgElements.Add(lineSerifBotSizeLengthOfAnchorWithoutRadius);

			lineVertSizeLengthOfAnchorWithoutRadius = GetSvgLineElement(m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin,
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40),
					m_YOrigin + m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters),
					Color.Black,
					0.5f,
					SvgLengthUnits.Pixels);

			svgElements.Add(lineVertSizeLengthOfAnchorWithoutRadius);

			svgElements.Add(GetSvgTextElement($"{anchor.LengthMillimeters - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters)}",
					m_XOrigin + (anchor.ThreadDiameterMillimeters + anchor.DiameterMillimeters) / 2 + (outPartHorSize + 40) - 2,
					m_YOrigin + (m_ScaledThreadLength + ANCHOR_LENGTH_MAX - (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters)) / 2 + 10,
					-90,
					SvgLengthUnits.Pixels)); // Make text of size's value LengthMillimeters of anchor without radius

			svgElements.Add(lineHorTopSizeLengthOfAnchor);

			svgElements.Add(lineHorBotSizeLengthOfAnchor);

			svgElements.Add(lineSerifBotSizeLengthOfAnchor);

			svgElements.Add(lineVertSizeLengthOfAnchor);

			svgElements.Add(lineSerifTopSizeLengthOfAnchor);
		}

		// GetDescriptionAnchor(anchor, paramsCanvas, svgElements);

		for (int i = 0; i < svgElements.Count; i++)
			svgDoc.RootSvg.Children.Insert(i, svgElements[i]);

		SvgViewBox view = new();
		view.MinX = 0;
		view.MinY = 0;
		view.Width = VIEW_WIDTH_PIXELS;
		view.Height = VIEW_HEIGHT_PIXELS;

		svgDoc.RootSvg.ViewBox = view;

		StringBuilder stringBuilder = new();
		svgDoc.Save(stringBuilder);
		string xml = stringBuilder.ToString();
		string svgElem = xml.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
		anchor.SvgElement = svgElem;
	}

	void GetScaledLength(float ThreadLengthMillimeters, float ThreadSecondLengthMillimeters)
	{
		m_ScaledThreadLength = ThreadLengthMillimeters;
		m_ScaledSecondThreadLength = ThreadSecondLengthMillimeters;
		if (ThreadLengthMillimeters >= ThreadSecondLengthMillimeters)
			GetConstraints(ThreadLengthMillimeters);
		else
			GetConstraints(ThreadSecondLengthMillimeters);
		void GetConstraints(float ThreadLengthMillimeters)
		{
			if (ThreadLengthMillimeters > 300 && ThreadLengthMillimeters <= 600)
			{
				m_ScaledThreadLength /= 2;
				m_ScaledSecondThreadLength /= 2;
			}
			else if (ThreadLengthMillimeters > 600 && ThreadLengthMillimeters <= 1000)
			{
				m_ScaledThreadLength /= 3f;
				m_ScaledSecondThreadLength /= 3f;
			}
			else if (ThreadLengthMillimeters > 1000 && ThreadLengthMillimeters <= 2500)
			{
				m_ScaledThreadLength /= 8f;
				m_ScaledSecondThreadLength /= 8f;
			}
			else if (ThreadLengthMillimeters > 2500 && ThreadLengthMillimeters <= 4000)
			{
				m_ScaledThreadLength /= 10f;
				m_ScaledSecondThreadLength /= 10f;
			}
			else if (ThreadLengthMillimeters > 4000 && ThreadLengthMillimeters <= 6000)
			{
				m_ScaledThreadLength /= 15f;
				m_ScaledSecondThreadLength /= 15f;
			}
		}
	}

	// Method for making text
	static SvgTextElement GetSvgTextElement(string content, float x, float y, float angle, SvgLengthUnits units)
	{
		var textSizeThreadLegth = new SvgContentElement
		{
			Content = $"{content}",
			Stroke = new SvgPaint(Color.Black),
			Color = new SvgColor(Color.Black),
		};

		var coordsXSizeThreadLegth = new List<SvgLength>();
		coordsXSizeThreadLegth.Add(new SvgLength(x, units));
		var coordsYSizeThreadLegth = new List<SvgLength>();
		coordsYSizeThreadLegth.Add(new SvgLength(y, units));

		var transforms = new List<SvgTransform>();
		var svgRotateTrans = new SvgRotateTransform()
		{
			Angle = new SvgAngle(angle),
			CenterX = new SvgLength(x),
			CenterY = new SvgLength(y),
		};
		transforms.Add(svgRotateTrans);

		SvgTextElement svgTextElement = new SvgTextElement
		{
			X = coordsXSizeThreadLegth,
			Y = coordsYSizeThreadLegth,
			Color = new SvgColor(Color.Black),
			FontStyle = SvgFontStyle.Normal,
			FontSize = new SvgLength(25, units),
			FontWeight = SvgFontWeight.Bold,
			Transform = transforms,
			TextOrientation = SvgTextOrientation.Mixed
		};

		svgTextElement.Children.Insert(0, textSizeThreadLegth);

		return svgTextElement;
	}

	// Method for making line
	static SvgLineElement GetSvgLineElement(float x1, float y1, float x2, float y2, Color color, float width, SvgLengthUnits units)
	{
		var svgLineElement = new SvgLineElement
		{
			X1 = new SvgLength(x1, units),
			Y1 = new SvgLength(y1, units),
			X2 = new SvgLength(x2, units),
			Y2 = new SvgLength(y2, units),
			Stroke = new SvgPaint(color),
			StrokeWidth = new SvgLength(width, units)
		};
		return svgLineElement;
	}

	// Method for making rectangle
	static SvgRectElement GetSvgRectElement(float x, float y, float width, float height, Color colorOfFill, Color colorOfStroke, float strokeWidth, SvgLengthUnits units)
	{
		var svgRectElement = new SvgRectElement
		{
			X = new SvgLength(x, units),
			Y = new SvgLength(y, units),
			Width = new SvgLength(width, units),
			Height = new SvgLength(height, units),
			Fill = new SvgPaint(colorOfFill),
			Stroke = new SvgPaint(colorOfStroke),
			StrokeWidth = new SvgLength(strokeWidth, units)
		};
		return svgRectElement;
	}

	// Method for making serif
	static SvgLineElement GetSerif(float x1, float y1, float x2, float y2, Color color, float width, SvgLengthUnits units)
	{
		var svgLineElement = new SvgLineElement
		{
			X1 = new SvgLength(x1 - 3, units),
			Y1 = new SvgLength(y1 + 3, units),
			X2 = new SvgLength(x2 + 3, units),
			Y2 = new SvgLength(y2 - 3, units),
			Stroke = new SvgPaint(color),
			StrokeWidth = new SvgLength(width, units)
		};
		return svgLineElement;
	}

	// Method for making serif for radius
	static SvgLineElement GetSerifRad(float x1, float y1, float x2, float y2, Color color, float width, SvgLengthUnits units)
	{
		var svgLineElement = new SvgLineElement
		{
			X1 = new SvgLength(x1 - 2, units),
			Y1 = new SvgLength(y1 + 5, units),
			X2 = new SvgLength(x2 + 2, units),
			Y2 = new SvgLength(y2 - 5, units),
			Stroke = new SvgPaint(color),
			StrokeWidth = new SvgLength(width, units)
		};
		return svgLineElement;
	}
}
