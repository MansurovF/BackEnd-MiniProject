$(document).ready(function () {
    
    $('.single-btn').click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        fetch(url)
            .then(res => {
                return res.text
            })
            .then(data => {
                console.log(data);
                $('.modal-content').html(data);

                $('.quick-view-image').slick({
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                    dots: false,
                    fade: true,
                    asNavFor: '.quick-view-thumb',
                    speed: 400,
                });

                $('.quick-view-thumb').slick({
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    asNavFor: '.quick-view-image',
                    dots: false,
                    arrows: false,
                    focusOnSelect: true,
                    speed: 400,
                });

            })
    })
    $('.search').keyup(function () {

        let search = $(this).val();
        let categoryId = $('.category-trigger').val();

        if (search.length >= 3) {
            fetch('/product/search?search=' + search + '&categoryId=' + categoryId)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.list-group').html(data)
                })
        } else {
            $('.list-group').html('');
        }

    })

    $('.addbasket').click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        fetch(url)
            .then(res => {
                return res.text();
            }).then(data => {
                $('.header-cart').html(data)
            })
    })

    $(document).on("click", ' .product-close , .element-delet, .basket-decrease', function (e){
        e.stopPropagation();
    e.preventDefault();
        console.log(e.currentTarget)

        if (e.currentTarget.classList.contains('.product-close') || e.currentTarget.classList.contains('.element-delet')) {
            let url = $(e.currentTarget).attr('href');

            fetch(url)
                .then(res => {
                    return res.text();
                }).then(data => {
                    $('.header-cart').html(data)
                    console.log(url)
                    let url2 = "/" + url.split('/')[1] + "/mainbasket"
                    console.log(url2)
                    fetch(url2)
                        .then(res2 => {
                            return res2.text()
                        })
                        .then(data2 => {
                            $('.cart-page').html(data2)
                        })
                })

        } else if (e.currentTarget.classList.contains('basket-increase') || e.currentTarget.classList.contains('basket-increase')) {
            let url = $(e.currentTarget).attr('href');
            console.log(url);
            fetch(url)
                .then(res => {
                    return res.text();
                }).then(data => {
                    $('.header-cart').html(data)
                    console.log(url)
                    let url2 = "/" + url.split('/')[1] + "/refreshbasketmain"
                    console.log(url2)
                    fetch(url2)
                        .then(res2 => {
                            return res2.text()
                        })
                        .then(data2 => {
                            $('.cart-page').html(data2)
                        })
                })
        }
    })
})
