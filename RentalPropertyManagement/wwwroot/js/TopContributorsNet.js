function topContributorNetPieChart(dataIN) {
    var dataINJSON = []
    dataINJSON = JSON.parse(dataIN)
    console.log("topContributorPieChart")
    console.log(dataINJSON)
    var pieData = [];
    var pieColor = ["#cce5e5", "#ffedce"]
    var i = 0;
    for (const e of dataINJSON) {
        pieData.push({ name: e.ShortName, y: e.Net, color: pieColor[i] })
        i++;
    }

    var chart = new CanvasJS.Chart("netPieChart", {
        animationEnabled: true,
        title: {
            text: "Net",
            fontSize: 26,
            fontFamily: "Lato",
            fontWeight: "bold"
        },
        legend: {
            cursor: "pointer",
            itemclick: explodePie,
            fontSize: 12,
        },
        data: [{
            type: "doughnut",
            innerRadius: 90,
            showInLegend: true,
            //indexLabelPlacement: "inside",
            //indexLabelFontWeight: "bold",
            toolTipContent: "<b>{name}</b>: ${y} (#percent%)",
            indexLabel: "{name} ${y}",
            dataPoints: pieData
        }]
    });
    chart.render();
    var dps = chart.options.data[0].dataPoints;
    var sum = 0;

    for (var i = 0; i < dps.length; i++) {

        sum += dps[i].y;

    }

    document.getElementById("totalNet").innerHTML = "$" + Math.round(sum);
}

function explodePie(e) {
    if (typeof (e.dataSeries.dataPoints[e.dataPointIndex].exploded) === "undefined" || !e.dataSeries.dataPoints[e.dataPointIndex].exploded) {
        e.dataSeries.dataPoints[e.dataPointIndex].exploded = true;
    } else {
        e.dataSeries.dataPoints[e.dataPointIndex].exploded = false;
    }
    e.chart.render();

}