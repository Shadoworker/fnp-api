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

        // console.log(auth_header)

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
              // console.log(claims)

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
            // console.log(claims)

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

        // Get Token
        try {

          let _url = CONSTANTS.playtix_request_base_url+'/v1/token?grant_type=client_credentials';
          const { data } = await axios.post(_url, undefined, config)

          var accessTokenString = data.access_token;
 
          // console.log(accessTokenString)

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


 

      async getGameRessources(ctx) { // l3v3l: get game resources list

        var url_string = CONSTANTS.api_url+ ctx.originalUrl;

        var url = new URL(url_string);

        var gameId = JSON.parse(url.searchParams.get("gameId"));

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

        
        // Get Token
        try {

          let _url = CONSTANTS.playtix_request_base_url+'/v1/token?grant_type=client_credentials';
          const { data } = await axios.post(_url, undefined, config)

          var accessTokenString = data.access_token;
 
          console.log(accessTokenString)

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

            let _url2 = CONSTANTS.playtix_api_base_url+'resources/game/'+gameId;
            
            const { data } = await axios.post(_url2, undefined, config2);

            console.log(data);
            ctx.body = data;
              
  
          } catch (err) {
            ctx.body = err;
          }
         

        } catch (err) {
          ctx.body = err;
        }
 


      },

      
      async getFish(ctx) { // get a random fish : with sent specs

            
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
              species: {
                populate: {
                  id_mapping: true,
                },
              },
              fish_sizes:true
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
                species: {
                  populate: {
                    id_mapping: true,
                  },
                },
                fish_sizes:true
              }
            }
            const entity = await strapi.service('api::fish.fish').find(query);

            fishes = entity["results"];
            r = Math.floor(Math.random() * fishes.length);

          }

        var selected_fish = fishes[r];

          // Getting fish sizes
        var sizes = selected_fish["fish_sizes"];
        var r_size = Math.floor(Math.random() * sizes.length);
        var size = sizes[r_size]["name"];
        var size_ratio = sizes[r_size]["ratio"];

        var fish_weight = selected_fish["species"]["base_weight"] * size_ratio;


        let returnedFish = {
          name : selected_fish["species"]["name"],
          size : size,
          weight: fish_weight
        }

        ctx.body = returnedFish;


      },



      async updatePlayerResource(ctx) { // l3v3l: update a specific resource for a player

        var url_string = CONSTANTS.api_url+ ctx.originalUrl;

        var url = new URL(url_string);

        // Params
        var playerId = JSON.parse(url.searchParams.get("playerId"));
        var resourceId = JSON.parse(url.searchParams.get("resourceId"));
        var resourceAmount = JSON.parse(url.searchParams.get("resourceAmount"));
        // ----------------------

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

        
        // Get Token
        try {

          let _url = CONSTANTS.playtix_request_base_url+'/v1/token?grant_type=client_credentials';
          const { data } = await axios.post(_url, undefined, config)

          var accessTokenString = data.access_token;
 
          // console.log(accessTokenString)

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

          
          let _body = {
            player_id: playerId,
            resources: [
              {
                resource_id: resourceId,
                amount: resourceAmount
              }
            ]
          }

          try {

            let _url2 = CONSTANTS.playtix_api_base_url+'player-resources';
            
            const { data } = await axios.post(_url2, _body, config2);

            // console.log(data);
            ctx.body = data;
              
  
          } catch (err) {
            ctx.body = err;
          }
         

        } catch (err) {
          ctx.body = err;
        }
 


      },



  }));
   