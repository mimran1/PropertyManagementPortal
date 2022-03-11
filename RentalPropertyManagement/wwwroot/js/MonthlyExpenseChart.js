function generateMonthlyExpenseChart(dataIN, showInner) {
    var dataINJSON = []
    dataINJSON = JSON.parse(dataIN)
    console.log('generateMonthlyExpenseChart' + dataINJSON)
    var pieData = [];
    dataINJSON.forEach(elem => pieData.push({ name: elem.Category, y: elem.Amount, y2: Math.round(elem.Amount) }))
    var chart = new CanvasJS.Chart("expensePieChart", {
        theme: "light2",
        exportFileName: "Doughnut Chart",
        exportEnabled: false,
        animationEnabled: true,
        title: {
            text: "Expense",
            fontSize: 26,
            fontFamily: "Lato"
        },
        legend: {
            verticalAlign: "bottom",  // "top" , "bottom"
            horizontalAlign: "center",
            cursor: "pointer",
            itemclick: explodePie
        },
        data: [{
            type: "doughnut",
            innerRadius: 90,
            showInLegend: true,
            toolTipContent: "<b>{name}</b>: ${y2} (#percent%)",
            //indexLabel: "{name} - #percent%",
            indexLabel: "{name} - ${y2}",
            dataPoints: pieData
        }]
    });
    chart.render();
    if (showInner) {
        var dps = chart.options.data[0].dataPoints;
        var sum = 0.0;
        for (var i = 0; i < dps.length; i++) {

            sum += dps[i].y;

        }
        document.getElementById("totalExpense").innerHTML = "$" + Math.round(sum);
    }
}

function explodePie(e) {
    if (typeof (e.dataSeries.dataPoints[e.dataPointIndex].exploded) === "undefined" || !e.dataSeries.dataPoints[e.dataPointIndex].exploded) {
        e.dataSeries.dataPoints[e.dataPointIndex].exploded = true;
    } else {
        e.dataSeries.dataPoints[e.dataPointIndex].exploded = false;
    }
    e.chart.render();
}
