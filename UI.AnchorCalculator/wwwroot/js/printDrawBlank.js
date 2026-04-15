function PrintDraw(id)
{
	$.ajax(
	{
		url: '/anchor/Anchor/GetAnchorJsonResult',
		type: 'GET',
		data: { 'id': id },
		success: function (response)
		{
			if (response.success)
			{
				console.log(response.anchor, 'production ' + response.anchor.productionId + '');

				let firstName = 'Шпилька';
				let form;
				let executor = response.anchor.user.userName;
				let billetLengthMillimeters = response.anchor.billetLengthMillimeters;
				let bendRadiusMillimeters = response.anchor.bendRadiusMillimeters;
				let quantity = response.anchor.quantity;
				let threadDiameterMillimeters = response.anchor.threadDiameterMillimeters;
				let lengthMillimeters = response.anchor.lengthMillimeters;
				let threadLengthMillimeters = response.anchor.threadLengthMillimeters;
				let materialFullName = response.anchor.material.fullName;
				let withoutBindThreadDiamMaterial = response.anchor.withoutBindThreadDiamMaterial;
				let dateNow = new Date();
				let date = dateNow.getDate().toString().padStart(2, '0');
				let month = (dateNow.getMonth() + 1).toString().padStart(2, '0');
				let year = dateNow.getFullYear().toString();
				let rollerPathLengthMillimetersBeforeEnd = response.anchor.rollerPathLengthMillimetersBeforeEnd;
				let productionStr = '';
				let withoutBindThreadDiamMaterialInfo = '';
				if (response.anchor.productionId === 0)
				{
					productionStr = 'Изготовить на токарном станке';
				}
				else if (response.anchor.productionId === 1)
					productionStr = 'Изготовить на резьбонакатном станке';
				else
					productionStr = 'Изготовить на гидравлическом станке';

				let dateNowFormat = date + '.' + month + '.' + year;

				let fingerCenter = "";
				let mandrelCenter = "";
				let fingerSide = "";
				let mandrelSide = "";
				if (bendRadiusMillimeters > 0 && bendRadiusMillimeters <= 12)
				{
					fingerCenter = "1";
					mandrelCenter = "_";
					fingerSide = "7";
					mandrelSide = "O11";
				}
				else if (bendRadiusMillimeters > 12 && bendRadiusMillimeters <= 16)
				{
					fingerCenter = "3";
					mandrelCenter = "_";
					fingerSide = "7";
					mandrelSide = "O10";
				}
				else if (bendRadiusMillimeters > 16 && bendRadiusMillimeters <= 20)
				{
					fingerCenter = "4";
					mandrelCenter = "_";
					fingerSide = "7";
					mandrelSide = "O9";
				}
				else if (bendRadiusMillimeters > 20 && bendRadiusMillimeters <= 24)
				{
					fingerCenter = "6";
					mandrelCenter = "_";
					fingerSide = "7";
					mandrelSide = "O6";
				}
				else if (bendRadiusMillimeters > 24 && bendRadiusMillimeters <= 30)
				{
					fingerCenter = "8";
					mandrelCenter = "_";
					fingerSide = "7";
					mandrelSide = "O1";
				}
				else if (bendRadiusMillimeters > 30 && bendRadiusMillimeters <= 36)
				{
					fingerCenter = "7";
					mandrelCenter = "O4";
					fingerSide = "4";
					mandrelSide = "_";
				}
				else if (bendRadiusMillimeters > 36 && bendRadiusMillimeters <= 42)
				{
					fingerCenter = "7";
					mandrelCenter = "O6";
					fingerSide = "7";
					mandrelSide = "O8";
				}
				else if (bendRadiusMillimeters > 42 && bendRadiusMillimeters <= 48)
				{
					fingerCenter = "7";
					mandrelCenter = "O8";
					fingerSide = "7";
					mandrelSide = "O3";
				}
				else
				{
					fingerCenter = "_";
					mandrelCenter = "_";
					fingerSide = "_";
					mandrelSide = "_";
				}

				let toleranceNum = 3;
				let productionNum = 5;
				let rollerPathLengthMillimetersBeforeEndP = '<p class="card-text" > 2. Длина до конца пути ролика ' + rollerPathLengthMillimetersBeforeEnd + ' мм</p>';
				let tooling = '<span class="mr-auto">4. Оснастка: центр: палец ' + fingerCenter + ', оправка: ' + mandrelCenter + '; бок: палец ' + fingerSide + ', оправка ' + mandrelSide + '';
				if (response.anchor.kind === 0)
				{
					toleranceNum = 2;
					productionNum = 3;
					rollerPathLengthMillimetersBeforeEndP = '';
					tooling = '';
					if (response.anchor.threadSecondLengthMillimeters > 0)
						form = 'прямая две резьбы';
					else
						form = 'прямая';
				}
				else if (response.anchor.kind === 1)
				{
					form = 'гнутая';
				}
				else
				{
					firstName = 'Хомут';
					form = 'прямоугольный';
				}

				if (withoutBindThreadDiamMaterial)
				{
					console.log('withoutBindThreadDiamMaterialInfo - ' + withoutBindThreadDiamMaterialInfo + '');
					withoutBindThreadDiamMaterialInfo = '<p class="card-text">' + (productionNum + 1) + '. Проточить до диаметра резьбы на токарном станке</p>';
				}

				let notes;
				if (threadLengthMillimeters > 0)
				{
					notes =
						'<div><p class="card-text fw-bold">Кол - во: ' + quantity + ' шт.</p>' +
						'<p class="card-text">1. Размер заготовки ' + billetLengthMillimeters + ' мм</p>' +
						rollerPathLengthMillimetersBeforeEndP +
						'<p class="card-text">' + toleranceNum + '. Поле допуска на диаметр резьбы 8q по ГОСТ 16093</p>' +
						'<p class="card-text"' +
						tooling + '</p>' +
						'<p class="card-text" > ' + productionNum + '. ' + productionStr + '</p>' + withoutBindThreadDiamMaterialInfo +'</div > ';
				}
				else
				{
					notes =
						'<div><p class="card-text fw-bold">Кол - во: ' + quantity + ' шт.</p>' +
						'<p class="card-text">1. Размер заготовки ' + billetLengthMillimeters + ' мм</p>' +
						rollerPathLengthMillimetersBeforeEndP +
						'<p class="card-text">' + toleranceNum + '. Поле допуска на диаметр резьбы 8q по ГОСТ 16093</p>' +
						'<p class="card-text"' +
						tooling + '</p></div > ';
				}

				let stamp = '<table class="table table-bordered border-dark">' +
					'<thead>' +
					'<tr>' +
					'<th colspan="2" class="text-center">' + firstName + ' M' + threadDiameterMillimeters + 'x' + length + ' ' + form + '</th>' +
					'<td class="text-left">Тип заказа:</td>' +
					'<td class="text-center">' + materialFullName + '</td>' +
					'</tr>' +
					'</thead>' +
					'<tbody>' +
					'<tr>' +
					'<th scope="col" class="w-25">Исполнитель</th>' +
					'<td class="w-30">' + executor + '</td>' +
					'<td class="w-25"></td>' +
					'<td class="text-center w-20">' + dateNowFormat + '</td>' +
					'</tr>' +
					'</tbody>' +
					'</table>';

				let drawing_pdf = document.getElementById("drawing_pdf");
				let notes_pdf = document.getElementById("notes_pdf");
				notes_pdf.innerHTML = notes;
				let stamp_pdf = document.getElementById("stamp_pdf");
				stamp_pdf.innerHTML = stamp;

				console.log(drawing_pdf);
				PrintPdf("drawing_pdf", 'Чертёж_'+ id +'');
			}
		}
	});
}

function PrintBlank(id)
{
	PrintPdf("blank_pdf", 'Просчёт_' + id + '');
}

function PrintPdf(id, name)
{
	var title = document.getElementById("title");
	title.innerText = name;
	var pdf = document.getElementById(''+id+'');
	pdf.style.display = "block";

	// Apply scaling
	pdf.style.transform = "scale(1)"; // Adjust the scale factor as needed
	pdf.style.transformOrigin = "top left";

	$('#' + id + '').printThis(
	{
		afterPrint: function ()
		{
			// Remove scaling and hide the element after printing
			pdf.style.transform = "";
			pdf.style.transformOrigin = "";
			pdf.style.display = "none";
		},
	});

	setTimeout(() =>
	{
		pdf.style.display = "none";
	}, 500);
}