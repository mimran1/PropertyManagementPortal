function topContributorPieChart(dataIN) {
    var dataINJSON = []
    dataINJSON = JSON.parse(dataIN)
    console.log("topContributorPieChart")
    console.log(dataINJSON)
    var pieData = [];
    var pieColor = ["#cce5e5", "#ffedce"]
    var i = 0;
    //Following code courtsey: https://stackoverflow.com/questions/149055/how-to-format-numbers-as-currency-strings
    // Create our number formatter.
    var formatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',

        // These options are needed to round to whole numbers if that's what you want.
        //minimumFractionDigits: 0, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
        maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
    });

    formatter.format(2500); /* $2,500.00 */
    for (const e of dataINJSON) {
        pieData.push({ name: e.ShortName, y: e.Income, color: pieColor[i] })
        i++;
    }

    var chart = new CanvasJS.Chart("chartContainer", {
        animationEnabled: true,
        title: {
            text: "Income",
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

    document.getElementById("totalIncome").innerHTML = formatter.format(Math.round(sum));
}

function explodePie(e) {
    if (typeof (e.dataSeries.dataPoints[e.dataPointIndex].exploded) === "undefined" || !e.dataSeries.dataPoints[e.dataPointIndex].exploded) {
        e.dataSeries.dataPoints[e.dataPointIndex].exploded = true;
    } else {
        e.dataSeries.dataPoints[e.dataPointIndex].exploded = false;
    }
    e.chart.render();

}