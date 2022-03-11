function generateMonthlyExpenseChart_2020(dataIN) {
    var dataINJSON = []
    dataINJSON = JSON.parse(dataIN)
    console.log('generateMonthlyExpenseChart_2020 **** ' + dataINJSON)
    var data_Expenditure = [];
    var data_Income = [];
    var data_Net = [];
    /*for (var i = 0; i < 5; i++) {
        data_Expenditure.push(
            {
                label: dataINJSON[i].Month,
                y: dataINJSON[i].Expenditure
            }
        )
    }*/
    dataINJSON.forEach(elem => data_Expenditure.push({ label: elem.Month, y: elem.Expenditure }))
    dataINJSON.forEach(elem => data_Income.push({ label: elem.Month, y: elem.Income }))
    dataINJSON.forEach(elem => data_Net.push({ label: elem.Month, y: elem.Net }))

    var chart = new CanvasJS.Chart("monthlyExpenseChart_2020", {
        theme: "light2", //gets rid of y-axis

        animationEnabled: true,  //change to false
        title: {
            text: "Monthly Performance",
            fontSize: 18,
            fontFamily: "Lato"
        },
        toolTip: {
            shared: false,
            content: toolTipFormatter
        },
        data: [
            {
                // Change type to "doughnut", "line", "splineArea", etc.
                type: "column",
                name: "Income",
                showInLegend: true,
                click: onClick,
                color: "#4fa1db",
                fillOpacity: .8,
                dataPoints: data_Income
            },
            {
                // Change type to "doughnut", "line", "splineArea", etc.
                type: "column",
                name: "Expenditure",
                showInLegend: true,
                click: onClick,
                color: "#e47e83",
                fillOpacity: .8,
                dataPoints: data_Expenditure
            },
            {
                // Change type to "doughnut", "line", "splineArea", etc.
                type: "column",
                name: "Net",
                showInLegend: true,
                click: onClick,
                color: "#4cab98",
                fillOpacity: .8,
                dataPoints: data_Net
            }
        ],
        /** Set axisY properties here*/
        axisY: {
            prefix: "$",
            margin: 20
        },
        axisX: {
            margin: 20
        }
    });

    chart.render();
    function onClick(e) {
        window.location.href = "/MonthlyFinancialDetail/2020/" + e.dataPoint.label + "";
    }
    function toolTipFormatter(e) {
        var str = "";
        var total = 0;
        var str3;
        var str2;
        for (var i = 0; i < e.entries.length; i++) {
            var str1 = "<span style= \"color:" + e.entries[i].dataSeries.color + "\">" + e.entries[i].dataSeries.name + "</span>: <strong>" + e.entries[i].dataPoint.y + "</strong> <br/>";
            total = e.entries[i].dataPoint.y + total;
            str = str.concat(str1);
        }
        str2 = "<strong>" + e.entries[0].dataPoint.label + "</strong> <br/>";
        //        str3 = "<span style = \"color:Tomato\">Total: </span><strong>" + total + "</strong><br/>";
        return (str2.concat(str));
    }
}