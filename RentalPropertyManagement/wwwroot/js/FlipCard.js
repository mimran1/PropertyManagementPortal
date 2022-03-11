function flipCard() {
    console.log('flipping card')
    var card = document.querySelector('.thecard');
    card.addEventListener('click', function () {
        card.classList.toggle('clicked');
    });
    
}