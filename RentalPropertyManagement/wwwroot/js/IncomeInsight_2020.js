function incomeInsight_2020(dataIN) {
    var dataINJSON = []
    dataINJSON = JSON.parse(dataIN)
    console.log("In incomeInsight_2020:\n")
    console.log(dataINJSON)
    var data_Room1A = []
    var data_Room1B = []
    var data_Room1C = []
    var data_Room1D = []
    var data_Room2A = []
    var data_Room2B = []
    var data_Room2C = []
    var data_Room2D = []
    var incomeForYear

    //dataINJSON.forEach(elem => { if (elem.Month == 'August') data_Aug.push({ label: elem.Category, y: elem.Amount })})

    dataINJSON.forEach(elem => {
        if (elem.Room == 'Room1A')
            data_Room1A.push({ label: elem.Month, y: elem.Amount })
        else if (elem.Room == 'Room1B')
            data_Room1B.push({ label: elem.Month, y: elem.Amount })
        else if (elem.Room == 'Room1C')
            data_Room1C.push({ label: elem.Month, y: elem.Amount })
        else if (elem.Room == 'Room1D')
            data_Room1D.push({ label: elem.Month, y: elem.Amount })
        else if (elem.Room == 'Room2A')
            data_Room2A.push({ label: elem.Month, y: elem.Amount })
        else if (elem.Room == 'Room2B')
            data_Room2B.push({ label: elem.Month, y: elem.Amount })
        else if (elem.Room == 'Room2C')
            data_Room2C.push({ label: elem.Month, y: elem.Amount })
        else if (elem.Room == 'Room2D')
            data_Room2D.push({ label: elem.Month, y: elem.Amount })
        incomeForYear = elem.Year
    }
    )


    var chart = new CanvasJS.Chart("incomeInsightChart_2020", {
        theme: "light2",
        animationEnabled: true,
        title: {
            text: "Income for " + incomeForYear
        },
        axisY: {
            prefix: "$",
            margin: 20
        },
        legend: {
            cursor: "pointer",
            itemclick: toggleDataSeries
        },
        toolTip: {
            shared: true,
            content: toolTipFormatter
        },
        data: [{
            type: "column",
            showInLegend: true,
            name: "Room1A",
            color: "red",
            dataPoints: data_Room1A
        },
        {
            type: "column",
            showInLegend: true,
            name: "Room1B",
            color: "#0479cc",
            dataPoints: data_Room1B
        },
        {
            type: "column",
            showInLegend: true,
            name: "Room1C",
            color: "#68d5c8",
            dataPoints: data_Room1C
        },
        {
            type: "column",
            showInLegend: true,
            name: "Room1D",
            color: "#e6c54f",
            dataPoints: data_Room1D
        },
        {
            type: "column",
            showInLegend: true,
            name: "Room2A",
            color: "#f9777f",
            dataPoints: data_Room2A
        },
        {
            type: "column",
            showInLegend: true,
            name: "Room2B",
            color: "#4db9f2",
            dataPoints: data_Room2B
        },
        {
            type: "column",
            showInLegend: true,
            name: "Room2C",
            color: "#ff6d41",
            dataPoints: data_Room2C
        },
        {
            type: "column",
            showInLegend: true,
            name: "Room2D",
            color: "#ff6d41",
            dataPoints: data_Room2D
        }

        ]
    });
    chart.render();

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
        str3 = "<span style = \"color:Tomato\">Total: </span><strong>" + total + "</strong><br/>";
        return (str2.concat(str)).concat(str3);
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