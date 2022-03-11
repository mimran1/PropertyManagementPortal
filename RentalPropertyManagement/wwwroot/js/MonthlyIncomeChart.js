function generateMonthlyIncomeChart(dataIN) {
	var dataINJSON = []
	dataINJSON = JSON.parse(dataIN)
	console.log("INCOME DATA")
	console.log(dataINJSON)
	var pieData = [];
	dataINJSON.forEach(elem => pieData.push({ name: elem.Room, y: elem.Amount }))
	
	var chart = new CanvasJS.Chart("incomePieChart", {
		theme: "light2",
		exportFileName: "Doughnut Chart",
		exportEnabled: true,
		animationEnabled: true,
		title: {
			text: "Income"
		},
		legend: {
			cursor: "pointer",
			itemclick: explodePie,
		},
		data: [{
			type: "doughnut",
			innerRadius: 90,
			showInLegend: true,
			toolTipContent: "<b>{name}</b>: ${y} (#percent%)",
			indexLabel: "{name} - #percent%",
			dataPoints: pieData
		}]
	});
	chart.render();

	function explodePie(e) {
		if (typeof (e.dataSeries.dataPoints[e.dataPointIndex].exploded) === "undefined" || !e.dataSeries.dataPoints[e.dataPointIndex].exploded) {
			e.dataSeries.dataPoints[e.dataPointIndex].exploded = true;
		} else {
			e.dataSeries.dataPoints[e.dataPointIndex].exploded = false;
		}
		e.chart.render();
	}
}