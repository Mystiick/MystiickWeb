Cookie = {
    get: function (name) {
        var output = document.cookie.
            split(";").
            filter(x => x.split("=")[0].trim() === name.trim()).
            map(x => x.split("=")[1])
            [0];

        return output;

    },
    set: function (name, value) {
        throw Error("Not yet implemented");
    }
};