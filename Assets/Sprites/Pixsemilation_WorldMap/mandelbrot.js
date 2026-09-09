(function () {
    var sprite = app.activeSprite;
    var image = app.activeImage;

    if (!sprite || !image) {
        console.log("Open or create a sprite first.");
        return;
    }

    var width = image.width;
    var height = image.height;

    // Mandelbrot viewing window
    var minRe = -2.0;
    var maxRe = 1.0;
    var minIm = -1.2;
    var maxIm = 1.2;

    var maxIterations = 64;

    console.log("Generating Mandelbrot fractal...");

    for (var y = 0; y < height; y++) {

        for (var x = 0; x < width; x++) {

            // Convert image coordinate into complex-plane coordinate
            var cr = minRe + (x / width) * (maxRe - minRe);
            var ci = minIm + (y / height) * (maxIm - minIm);

            var zr = 0.0;
            var zi = 0.0;

            var iteration = 0;

            // z = z² + c
            while (
                (zr * zr + zi * zi) <= 4.0 &&
                iteration < maxIterations
            ) {
                var newZr =
                    (zr * zr) -
                    (zi * zi) +
                    cr;

                var newZi =
                    (2.0 * zr * zi) +
                    ci;

                zr = newZr;
                zi = newZi;

                iteration++;
            }

            var color;

            if (iteration === maxIterations) {

                // Mandelbrot set itself
                color = app.pixelColor.rgba(
                    0,
                    0,
                    0,
                    255
                );

            } else {

                // Grayscale exterior
                var brightness =
                    Math.floor(
                        255 -
                        ((iteration / maxIterations) * 255)
                    );

                color = app.pixelColor.rgba(
                    brightness,
                    brightness,
                    brightness,
                    255
                );
            }

            image.putPixel(
                x,
                y,
                color
            );
        }

        // Some progress feedback
        if (y % 100 === 0) {
            console.log(
                "Row " + y + " / " + height
            );
        }
    }

    sprite.commit();

    console.log("Fractal generation complete.");
})();