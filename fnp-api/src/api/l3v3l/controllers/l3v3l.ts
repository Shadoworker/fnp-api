/**
 * l3v3l controller
 */

import { factories } from '@strapi/strapi'
import axios from 'axios'; 

import {btoa} from 'buffer'; 
import fish from '../../fish/services/fish';
import CONSTANTS from '../content-types/constants';
import view from '../content-types/view';

const open = require("open")

const OktaJwtVerifier = require('@okta/jwt-verifier');


export default factories.createCoreController('api::l3v3l.l3v3l', ({ strapi }) =>  ({


    async callback(ctx) { // l3v3l MAIN CALLBACK : called by GET /callback

 
        var url_string = CONSTANTS.api_url+ ctx.originalUrl;

        var url = new URL(url_string);
        var code = url.searchParams.get("code");

        // console.log(code);

        let client_id = CONSTANTS.client_id;
        let client_secret = CONSTANTS.client_secret;

        const auth_header = btoa(`${client_id}:${client_secret}`);


        let config = {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/x-www-form-urlencoded',
            Authorization: 'Basic '+ auth_header
          }
         }

        var body = CONSTANTS.grant_url(CONSTANTS.api_url, CONSTANTS.api_path, code);
        
        // Get Token
        try {

          let _url = CONSTANTS.playtix_base_url+'/v1/token';
          const { data } = await axios.post(_url, body, config)

          var accessTokenString = data.id_token;

          const oktaJwtVerifier = new OktaJwtVerifier({
            issuer: CONSTANTS.playtix_base_url // required
          });

          // Get Player_ID
          try {
            
              let jwt = await oktaJwtVerifier.verifyAccessToken(accessTokenString, CONSTANTS.client_id)

              // console.log(jwt)
              // Return values
              var claims = JSON.stringify(jwt.claims);
              console.log(claims)

              ctx.body = view(accessTokenString, claims);
  
            } catch (err) {
              ctx.body = err;
            }
         

        } catch (err) {
          ctx.body = err;
        }
 


      },

    
      async getUserData(ctx) { // User data using token

 
        var url_string = CONSTANTS.api_url+ ctx.originalUrl;

        var url = new URL(url_string);
        var accessTokenString = url.searchParams.get("id_token");
      
 
        const oktaJwtVerifier = new OktaJwtVerifier({
          issuer: CONSTANTS.playtix_base_url // required
        });

        // Get Player_ID
        try {
          
            let jwt = await oktaJwtVerifier.verifyAccessToken(accessTokenString, CONSTANTS.client_id)

            // console.log(jwt)
            // Return values
            var claims = JSON.stringify(jwt.claims);
            console.log(claims)

            ctx.body = claims;

          } catch (err) {
            ctx.body = err;
          }
 


      },

 

      async getGames(ctx) { // l3v3l: get games list

        let client_id = CONSTANTS.requests_client_id;
        let client_secret = CONSTANTS.requests_client_secret;

        const auth_header = btoa(`${client_id}:${client_secret}`);

        let config = {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/x-www-form-urlencoded',
            Authorization: 'Basic '+ auth_header
          }
         }

        var body = null;
        
        // Get Token
        try {

          let _url = CONSTANTS.playtix_request_base_url+'/v1/token?grant_type=client_credentials';
          const { data } = await axios.post(_url, undefined, config)

          var accessTokenString = data.access_token;
 
          // ctx.body = accessTokenString;
          // Get games ...
          let config2 = {
            headers: {
              'Accept-Encoding' : 'application/json',
              'Accept': 'application/json',
              'Content-Type': 'application/json;charset=utf-8',
              Authorization: 'Bearer '+ accessTokenString
            }
           }
          try {

            let _url2 = CONSTANTS.playtix_api_base_url+'games';
            
            const { data } = await axios.get(_url2, config2);

            console.log(data);
            ctx.body = data;
              
  
          } catch (err) {
            ctx.body = err;
          }
         

        } catch (err) {
          ctx.body = err;
        }
 


      },


      
      async getAFish(ctx) { // get a random fish : with sent specs

            
          var url_string = CONSTANTS.api_url+ ctx.originalUrl;

          var url = new URL(url_string);

          var areas = JSON.parse(url.searchParams.get("areas"));
          var rods = JSON.parse(url.searchParams.get("rods"));

          // console.log(areas[0]);
          // console.log(rods);


          const query = 
          {
            where:
            {
            },
            populate: {
              espece: {
                populate: {
                  id_mapping: true,
                },
              },
            }
          }
        
          const entity = await strapi.service('api::fish.fish').find(query);

          var fishes :object[] = entity["results"];

          var r = Math.floor(Math.random() * fishes.length);

          if(fishes.length == 0){
          
            const query = 
            {
              where:
              {
              },
              populate: {
                espece: {
                  populate: {
                    id_mapping: true,
                  },
                },
              }
            }
            const entity = await strapi.service('api::fish.fish').find(query);

            fishes = entity["results"];
            r = Math.floor(Math.random() * fishes.length);

          }

        var selected_fish = fishes[r];

          // Getting fish sizes
        var sizes = strapi.getModel('api::fish.fish').attributes.size.enum;
        var r_size = Math.floor(Math.random() * sizes.length);
        var size = sizes[r_size];

        selected_fish["size"] = size;


        ctx.body = selected_fish;


      },


  }));
   