const view = function(_token:string, _claims:any){

    return `
            <html>
                <head>
                <meta http-equiv=Content-Type content="text/html; charset=utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1" />

                </head>
                <style>
                  .box
                  {
                    display:flex; 
                    width:100%;
                    align-items:center;
                    justify-content:center;
                    flex-direction : column;
                  }
                  .title
                  {
                    font-family : Arial;
                  }
                  .btn {
                    display: block;
                    height: 25px;
                    background: #4E9CAF;
                    padding: 10px;
                    text-align: center;
                    border-radius: 5px;
                    color: white;
                    font-weight: bold;
                    line-height: 25px;
                    text-decoration:none;
                    font-family:Arial;
                    font-size:14px;
                    padding:8px 20px;
                  }
                </style>
                <body >
                <div class="box">
                <h3 class="title" >Connexion établie !</h3>
                <span>Votre token :  </span>
                <div style=" width: 80%; max-height: 250px;font-weight:bold; overflow-y:scroll ; margin: 10px 0px;"> 
                    ${_token} 
                </div>
                <a id="copy" class="btn" style="margin:10px" href="javascript:navigator.clipboard.writeText('${_token}');alert('Copied !')" >COPY TOKEN</a>
                <a id="btn" class="btn" href='fnpdl://signinLink?${_claims}'>RETOURNER AU JEU</a>
                </div>
 
                <script>
                
                  navigator.clipboard.writeText('${_token}').then(function() {
                     
                  }, function(err) {
                    
                  });

                </script>

                </body>
            </html>
            `;
 
}

export default view;