package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ListView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.adapters.PairAdapter;
import com.ramanco.samandroid.api.dtos.MosqueDto;
import com.ramanco.samandroid.api.endpoints.MosquesApiEndpoint;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;
import com.ramanco.samandroid.utils.UxUtil;
import com.rey.material.widget.EditText;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import retrofit2.Response;

public class MosqueSelectionFragment extends Fragment {

    //region Fields:
    ProgressDialog progress;
    SendConsolationFragment parentView;
    MosqueDto[] allMosques;
    //endregion

    //region Ctors:
    public MosqueSelectionFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, final ViewGroup container,
                             Bundle savedInstanceState) {
        View fragmentView = inflater.inflate(R.layout.fragment_mosque_selection, container, false);

        try {

            loadMosquesAsync();

            //region set on item click listener:
            ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
            lvItems.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                @Override
                public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                    try {
                        KeyValuePair pair = (KeyValuePair) parent.getItemAtPosition(position);
                        MosqueDto mosque = (MosqueDto) pair.getTag();
                        if (parentView != null) {
                            parentView.setSelectedMosque(mosque);
                            parentView.showObitSelectionStep();
                        }
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

            //region set on text change listener:
            EditText etSearch = (EditText) fragmentView.findViewById(R.id.et_search);
            etSearch.addTextChangedListener(new TextWatcher() {
                @Override
                public void beforeTextChanged(CharSequence s, int start, int count, int after) {

                }

                @Override
                public void onTextChanged(CharSequence s, int start, int before, int count) {
                }

                @Override
                public void afterTextChanged(Editable s) {
                    try {
                        filterMosques(s.toString());
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

            //region nav buttons:
            parentView.setNextVisible(true);
            parentView.setOnNextClickListener(new Runnable() {
                @Override
                public void run() {
                    try {
                        parentView.setSelectedMosque(null);
                        parentView.showObitSelectionStep();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });

            parentView.setPrevVisible(false);
            //endregion

        } catch (Exception ex) {
            if (progress != null)
                progress.dismiss();
            ExceptionManager.handle(getActivity(), ex);
        }

        return fragmentView;
    }
    //endregion

    //region Getters & Setters:

    public SendConsolationFragment getParentView() {
        return parentView;
    }

    public void setParentView(SendConsolationFragment parentView) {
        this.parentView = parentView;
    }

    //endregion

    //region Methods:
    private void loadMosquesAsync() throws IOException, CallServerException {

        //region call server and fill list view:
        final int cityId = PrefUtil.getCityID(getActivity());
        progress = UxUtil.showProgress(getActivity());
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    MosquesApiEndpoint endpoint = ApiUtil.createEndpoint(MosquesApiEndpoint.class);
                    Response<MosqueDto[]> response = endpoint.findByCity(cityId).execute();
                    if (!response.isSuccessful())
                        throw new CallServerException(getActivity());
                    allMosques = response.body();
                    //region fill list view:
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                KeyValuePair[] pairs = toPairs(allMosques);
                                View fragmentView = getView();
                                if (fragmentView != null) {
                                    fillListView(fragmentView, pairs);
                                    progress.dismiss();
                                }
                            } catch (Exception ex) {
                                progress.dismiss();
                                ExceptionManager.handle(getActivity(), ex);
                            }
                        }
                    });
                    //endregion
                } catch (Exception ex) {
                    progress.dismiss();
                    ExceptionManager.handle(getActivity(), ex);
                }
            }
        }).start();
        //endregion

    }

    private void filterMosques(String name) {
        View fragmentView = getView();
        if (fragmentView != null) {
            // filter mosques:
            List<MosqueDto> filteredMosques = new ArrayList<>();
            for (MosqueDto mosque : allMosques) {
                if (mosque.getName().contains(name) || name.contains(mosque.getName())) {
                    filteredMosques.add(mosque);
                }
            }
            // load list view:
            KeyValuePair[] pairs = toPairs(filteredMosques.toArray(new MosqueDto[filteredMosques.size()]));
            fillListView(fragmentView, pairs);

        }
    }

    private KeyValuePair[] toPairs(MosqueDto[] mosques) {
        KeyValuePair[] pairs = new KeyValuePair[mosques.length];
        for (int i = 0; i < mosques.length; i++) {
            MosqueDto m = mosques[i];
            KeyValuePair pair = new KeyValuePair(Integer.toString(m.getId()), m.getName());
            pair.setDesc(String.format("%s: %s",
                    getActivity().getResources().getString(R.string.address),
                    m.getAddress()));
            pair.setTag(m);
            pairs[i] = pair;
        }
        return pairs;
    }

    private void fillListView(View fragmentView, KeyValuePair[] pairs) {
        PairAdapter adapter = new PairAdapter(getActivity(), pairs);
        ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
        lvItems.setAdapter(adapter);
    }
    //endregion

}
