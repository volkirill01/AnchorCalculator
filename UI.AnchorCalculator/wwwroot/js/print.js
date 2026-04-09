function printDiv(elemId)
{
	const body = document.body;
	const printContents = elemId.innerHTML;
	const originalContents = body.innerHTML;
	body.innerHTML = printContents;
	console.log(body);
	window.print();
	body.innerHTML = originalContents;
}
