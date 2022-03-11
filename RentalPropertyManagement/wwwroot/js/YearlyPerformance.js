function yearlyPerformanceLineChart(dataIN) {
	console.log('yearlkyperf')
	var dataINJSON = []
	dataINJSON = JSON.parse(dataIN)
	var barChartData47Floaral = [];
	var barChartData198WEnd = [];
	var datapointsData = [];
	var pieColor = ["#cce5e5", "#ffedce"]
	var i = 0;
	
	barChartData198WEnd.push({ y: 0, label: 2020 }) //hardcode 2020 for 198 so graph looks OK
	for (var i = 0; i < dataINJSON.length; i++) {
		if (dataINJSON[i].ShortName == '47 Floral')
			barChartData47Floaral.push({ y: dataINJSON[i].NetIncome, label: dataINJSON[i].Year })
		if (dataINJSON[i].ShortName == '198 W End')
			barChartData198WEnd.push({ y: dataINJSON[i].NetIncome, label: dataINJSON[i].Year })
	}
	
	var chart = new CanvasJS.Chart("yearlyPerformance", {
		theme: "light2",
		animationEnabled: true,
		title: {
			text: "Net Income",
			fontSize: 26,
			fontFamily: "Lato"
		},
		axisY: {
			prefix: "$",
			includeZero: true,
			titleFontSize: 18
		},
		legend: {
			verticalAlign: "bottom",  // "top" , "bottom"
			horizontalAlign: "center",
			cursor: "pointer",
			itemclick: toggleDataSeries,
			fontSize: 12,
		},
		toolTip: {
			content: toolTipFormatter
        },
		data: [{
			type: "stackedBar",
			showInLegend: true,
			color: "#cce5e5",
			name: "47 Floral",
			dataPoints: barChartData47Floaral
		},
		{
			type: "stackedBar",
			showInLegend: true,
			color: "#ffedce",
			name: "198 W End",
			dataPoints: barChartData198WEnd
		}]
	});
	chart.render();
	function toolTipFormatter(e) {
		var str = "";
		var total = 0;
		var str3;
		var str2;
		for (var i = 0; i < e.entries.length; i++) {
			var str1 = "<strong style= \"color:black\">" + e.entries[i].dataSeries.name + "</strong>: <span>" + "$" + e.entries[i].dataPoint.y + "</span> <br/>";
			total = e.entries[i].dataPoint.y + total;
			str = str.concat(str1);
		}
		str2 = "<strong>" + e.entries[0].dataPoint.label + "</strong> <br/>";
		
		return (str2.concat(str));
	}

	function toggleDataSeries(e) {
		if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
			e.dataSeries.visible = false;
		}
		else {
			e.dataSeries.visible = true;
		}
		chart.render();
	}


}